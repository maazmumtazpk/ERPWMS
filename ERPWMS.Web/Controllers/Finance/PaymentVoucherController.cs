using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers.Finance
{
    [Authorize]
    public class PaymentVoucherController : Controller
    {
        private readonly AppDbContext _context;

        public PaymentVoucherController(AppDbContext c)
        {
            _context = c;
        }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        [HttpGet]
        public IActionResult Index() => View();

        // -------------------------------------------------------
        // STATISTICS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.PaymentVouchers.Where(p => p.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(p => p.Status == "Paid" || p.Status == "Approved"),
                inactive = l.Count(p => p.Status == "Pending" || p.Status == "Draft"),
                addedThisMonth = l.Count(p =>
                    p.CreatedDate.HasValue &&
                    p.CreatedDate.Value.Month == DateTime.Now.Month &&
                    p.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.PaymentVouchers
                .Include(p => p.Supplier)
                .Include(p => p.BankAccount)
                .Where(p => p.IsDeleted != true)
                .OrderBy(p => p.PaymentId)
                .Select(p => new
                {
                    paymentId = p.PaymentId,
                    paymentNo = p.PaymentNo,
                    paymentDate = p.PaymentDate,
                    supplierName = p.Supplier != null ? p.Supplier.SupplierName : "N/A",
                    bankAccount = p.BankAccount != null ? p.BankAccount.AccountTitle : "N/A",
                    amount = p.Amount,
                    paymentMethod = p.PaymentMethod ?? "Cash",
                    status = p.Status ?? "Pending",
                    createdDate = p.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // GET ACCOUNTS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAccounts()
        {
            var l = await _context.ChartOfAccounts
                .Where(a => a.IsDeleted != true)
                .OrderBy(a => a.AccountCode)
                .Select(a => new
                {
                    accountId = a.AccountId,
                    accountCode = a.AccountCode,
                    accountName = a.AccountName,
                    accountType = a.AccountType
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // GET SUPPLIERS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetSuppliers()
        {
            var l = await _context.Suppliers
                .Where(s => s.IsActive == true && s.IsDeleted != true)
                .Select(s => new { supplierId = s.SupplierId, supplierName = s.SupplierName })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // GET BANKS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetBanks()
        {
            var l = await _context.BankAccounts
                .Where(b => b.IsActive == true && b.IsDeleted != true)
                .Select(b => new
                {
                    bankAccountId = b.BankAccountId,
                    accountTitle = b.AccountTitle,
                    accountNo = b.AccountNo
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // NEXT PAYMENT NO
        // -------------------------------------------------------
        private async Task<string> GenerateNextPaymentNo()
        {
            try
            {
                var year = DateTime.Now.Year;
                var prefix = "PV-" + year + "-";
                var existing = await _context.PaymentVouchers
                    .Where(p => p.PaymentNo != null && p.PaymentNo.StartsWith(prefix))
                    .Select(p => p.PaymentNo)
                    .ToListAsync();

                int maxNum = 0;
                foreach (var no in existing)
                {
                    if (no == null) continue;
                    var parts = no.Split('-');
                    if (parts.Length >= 3 && int.TryParse(parts[2], out int n) && n > maxNum)
                        maxNum = n;
                }
                return prefix + (maxNum + 1).ToString("D3");
            }
            catch
            {
                return "PV-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmss");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNextPaymentNo()
        {
            return Json(new { paymentNo = await GenerateNextPaymentNo() });
        }

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> AddPaymentPartial()
        {
            ViewBag.NextPaymentNo = await GenerateNextPaymentNo();
            return PartialView("_AddPaymentForm");
        }

        [HttpGet]
        public async Task<IActionResult> EditPaymentPartial(int id)
        {
            var p = await _context.PaymentVouchers
                .Include(x => x.PaymentVoucherDetails)
                .FirstOrDefaultAsync(x => x.PaymentId == id);
            if (p == null) return NotFound();
            return PartialView("_EditPaymentForm", p);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string paymentNo,
            DateTime? paymentDate,
            int? supplierId,
            int bankAccountId,
            string? paymentMethod,
            string? referenceNo,
            string? description)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paymentNo))
                    paymentNo = await GenerateNextPaymentNo();
                else if (await _context.PaymentVouchers.AnyAsync(p => p.PaymentNo == paymentNo && p.IsDeleted != true))
                    paymentNo = await GenerateNextPaymentNo();

                var f = Request.Form;
                var details = new List<PaymentVoucherDetail>();
                decimal totalDebit = 0, totalCredit = 0;

                var keys = f.Keys.Where(k => k.EndsWith(".AccountId")).ToList();

                foreach (var key in keys)
                {
                    var baseName = key.Replace(".AccountId", "");
                    var accStr = f[key].ToString();

                    if (!int.TryParse(accStr, out int accId) || accId <= 0)
                        continue;

                    decimal debit = 0, credit = 0;
                    decimal.TryParse(f[baseName + ".Debit"].ToString(), out debit);
                    decimal.TryParse(f[baseName + ".Credit"].ToString(), out credit);
                    var narration = f[baseName + ".Narration"].ToString() ?? "";

                    if (debit == 0 && credit == 0)
                        continue;

                    details.Add(new PaymentVoucherDetail
                    {
                        AccountId = accId,
                        Debit = debit,
                        Credit = credit,
                        Narration = narration,
                        CreatedBy = Uid().ToString(),
                        CreatedDate = DateTime.Now
                    });

                    totalDebit += debit;
                    totalCredit += credit;
                }

                if (details.Count < 2)
                    return Json(new { success = false, message = "At least 2 entry lines required." });

                if (totalDebit != totalCredit)
                    return Json(new { success = false, message = "Debit and Credit must be equal." });

                var voucher = new PaymentVoucher
                {
                    PaymentNo = paymentNo.Trim(),
                    PaymentDate = paymentDate ?? DateTime.Now,
                    SupplierId = supplierId,
                    BankAccountId = bankAccountId,
                    Amount = totalDebit,
                    PaymentMethod = paymentMethod ?? "Cash",
                    ReferenceNo = referenceNo,
                    Description = description ?? "",
                    Status = "Draft",
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                };
                _context.PaymentVouchers.Add(voucher);
                await _context.SaveChangesAsync();

                foreach (var dt in details)
                {
                    dt.PaymentId = voucher.PaymentId;
                    _context.PaymentVoucherDetails.Add(dt);
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Payment voucher created: " + voucher.PaymentNo });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // -------------------------------------------------------
        // EDIT
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int paymentId,
            string paymentNo,
            DateTime? paymentDate,
            int? supplierId,
            int bankAccountId,
            string? paymentMethod,
            string? referenceNo,
            string? description,
            string? status)
        {
            try
            {
                var p = await _context.PaymentVouchers
                    .Include(x => x.PaymentVoucherDetails)
                    .FirstOrDefaultAsync(x => x.PaymentId == paymentId);
                if (p == null)
                    return Json(new { success = false, message = "Not found" });

                p.PaymentNo = paymentNo.Trim();
                p.PaymentDate = paymentDate ?? p.PaymentDate;
                p.SupplierId = supplierId;
                p.BankAccountId = bankAccountId;
                p.PaymentMethod = paymentMethod;
                p.ReferenceNo = referenceNo;
                p.Description = description;
                p.Status = status;
                p.UpdatedBy = Uid().ToString();
                p.UpdatedDate = DateTime.Now;

                if (p.PaymentVoucherDetails != null && p.PaymentVoucherDetails.Count > 0)
                    _context.PaymentVoucherDetails.RemoveRange(p.PaymentVoucherDetails);

                var f = Request.Form;
                decimal totalDebit = 0, totalCredit = 0;
                var keys = f.Keys.Where(k => k.EndsWith(".AccountId")).ToList();

                foreach (var key in keys)
                {
                    var baseName = key.Replace(".AccountId", "");
                    var accStr = f[key].ToString();

                    if (!int.TryParse(accStr, out int accId) || accId <= 0)
                        continue;

                    decimal debit = 0, credit = 0;
                    decimal.TryParse(f[baseName + ".Debit"].ToString(), out debit);
                    decimal.TryParse(f[baseName + ".Credit"].ToString(), out credit);
                    var narration = f[baseName + ".Narration"].ToString() ?? "";

                    if (debit == 0 && credit == 0)
                        continue;

                    _context.PaymentVoucherDetails.Add(new PaymentVoucherDetail
                    {
                        PaymentId = paymentId,
                        AccountId = accId,
                        Debit = debit,
                        Credit = credit,
                        Narration = narration,
                        CreatedBy = Uid().ToString(),
                        CreatedDate = DateTime.Now
                    });

                    totalDebit += debit;
                    totalCredit += credit;
                }
                p.Amount = totalDebit;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Payment updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _context.PaymentVouchers
                .Include(x => x.PaymentVoucherDetails)
                .FirstOrDefaultAsync(x => x.PaymentId == id);
            if (p == null)
                return Json(new { success = false, message = "Not found" });

            if (p.PaymentVoucherDetails != null)
            {
                foreach (var d in p.PaymentVoucherDetails)
                    _context.PaymentVoucherDetails.Remove(d);
            }

            p.IsDeleted = true;
            p.DeletedBy = Uid().ToString();
            p.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }

        // -------------------------------------------------------
        // TOGGLE (PENDING/DRAFT -> PAID)
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var p = await _context.PaymentVouchers.FindAsync(id);
            if (p == null)
                return Json(new { success = false, message = "Not found" });

            var currentUid = Uid().ToString();
            if (p.CreatedBy == currentUid)
                return Json(new { success = false, message = "You cannot approve your own voucher." });

            if (p.Status == "Pending" || p.Status == "Draft")
            {
                p.Status = "Paid";
                p.ApprovedBy = Uid().ToString();
                p.ApprovedDate = DateTime.Now;
            }
            else
            {
                p.Status = "Pending";
                p.ApprovedBy = null;
                p.ApprovedDate = null;
            }
            p.UpdatedBy = Uid().ToString();
            p.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Status: " + p.Status });
        }

        // -------------------------------------------------------
        // PROFILE
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var p = await _context.PaymentVouchers
                .Include(x => x.PaymentVoucherDetails)
                .ThenInclude(d => d.Account)
                .Include(x => x.Supplier)
                .Include(x => x.BankAccount)
                .FirstOrDefaultAsync(x => x.PaymentId == id);
            if (p == null) return NotFound();

            var c = await _context.Users.FindAsync(p.CreatedBy);
            ViewBag.CreatedByName = c != null ? c.UserName : "N/A";

            // âœ… ApprovedBy/UpdatedBy string hain
            if (!string.IsNullOrWhiteSpace(p.ApprovedBy))
            {
                var a = await _context.Users.FindAsync(p.ApprovedBy);
                ViewBag.ApprovedByName = a != null ? a.UserName : "N/A";
            }
            else
            {
                ViewBag.ApprovedByName = "-";
            }

            if (!string.IsNullOrWhiteSpace(p.UpdatedBy))
            {
                var u = await _context.Users.FindAsync(p.UpdatedBy);
                ViewBag.UpdatedByName = u != null ? u.UserName : "N/A";
            }
            else
            {
                ViewBag.UpdatedByName = "-";
            }

            return View("Profile", p);
        }
    }
}
