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
    public class ReceiptVoucherController : Controller
    {
        private readonly AppDbContext _context;

        public ReceiptVoucherController(AppDbContext c)
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
            var l = await _context.ReceiptVouchers.Where(r => r.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(r => r.Status == "Received" || r.Status == "Approved"),
                inactive = l.Count(r => r.Status == "Pending" || r.Status == "Draft"),
                addedThisMonth = l.Count(r =>
                    r.CreatedDate.HasValue &&
                    r.CreatedDate.Value.Month == DateTime.Now.Month &&
                    r.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.ReceiptVouchers
                .Include(r => r.Customer)
                .Include(r => r.BankAccount)
                .Where(r => r.IsDeleted != true)
                .OrderBy(r => r.ReceiptId)
                .Select(r => new
                {
                    receiptId = r.ReceiptId,
                    receiptNo = r.ReceiptNo,
                    receiptDate = r.ReceiptDate,
                    customerName = r.Customer != null ? r.Customer.CustomerName : "N/A",
                    bankAccount = r.BankAccount != null ? r.BankAccount.AccountTitle : "N/A",
                    amount = r.Amount,
                    paymentMethod = r.PaymentMethod ?? "Cash",
                    status = r.Status ?? "Pending",
                    createdDate = r.CreatedDate
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
        // GET CUSTOMERS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetCustomers()
        {
            var l = await _context.Customers
                .Where(c => c.IsActive == true && c.IsDeleted != true)
                .Select(c => new { customerId = c.CustomerId, customerName = c.CustomerName })
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
        // NEXT RECEIPT NO
        // -------------------------------------------------------
        private async Task<string> GenerateNextReceiptNo()
        {
            try
            {
                var year = DateTime.Now.Year;
                var prefix = "RV-" + year + "-";
                var existing = await _context.ReceiptVouchers
                    .Where(r => r.ReceiptNo != null && r.ReceiptNo.StartsWith(prefix))
                    .Select(r => r.ReceiptNo)
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
                return "RV-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmss");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetNextReceiptNo()
        {
            return Json(new { receiptNo = await GenerateNextReceiptNo() });
        }

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> AddReceiptPartial()
        {
            ViewBag.NextReceiptNo = await GenerateNextReceiptNo();
            return PartialView("_AddReceiptForm");
        }

        [HttpGet]
        public async Task<IActionResult> EditReceiptPartial(int id)
        {
            var r = await _context.ReceiptVouchers
                .Include(x => x.ReceiptVoucherDetails)
                .FirstOrDefaultAsync(x => x.ReceiptId == id);
            if (r == null) return NotFound();
            return PartialView("_EditReceiptForm", r);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            string receiptNo,
            DateTime? receiptDate,
            int? customerId,
            int bankAccountId,
            string? paymentMethod,
            string? referenceNo,
            string? description)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(receiptNo))
                    receiptNo = await GenerateNextReceiptNo();
                else if (await _context.ReceiptVouchers.AnyAsync(r => r.ReceiptNo == receiptNo && r.IsDeleted != true))
                    receiptNo = await GenerateNextReceiptNo();

                var f = Request.Form;
                var details = new List<ReceiptVoucherDetail>();
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

                    details.Add(new ReceiptVoucherDetail
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

                var voucher = new ReceiptVoucher
                {
                    ReceiptNo = receiptNo.Trim(),
                    ReceiptDate = receiptDate ?? DateTime.Now,
                    CustomerId = customerId,
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
                _context.ReceiptVouchers.Add(voucher);
                await _context.SaveChangesAsync();

                foreach (var dt in details)
                {
                    dt.ReceiptId = voucher.ReceiptId;
                    _context.ReceiptVoucherDetails.Add(dt);
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Receipt voucher created: " + voucher.ReceiptNo });
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
            int receiptId,
            string receiptNo,
            DateTime? receiptDate,
            int? customerId,
            int bankAccountId,
            string? paymentMethod,
            string? referenceNo,
            string? description,
            string? status)
        {
            try
            {
                var r = await _context.ReceiptVouchers
                    .Include(x => x.ReceiptVoucherDetails)
                    .FirstOrDefaultAsync(x => x.ReceiptId == receiptId);
                if (r == null)
                    return Json(new { success = false, message = "Not found" });

                r.ReceiptNo = receiptNo.Trim();
                r.ReceiptDate = receiptDate ?? r.ReceiptDate;
                r.CustomerId = customerId;
                r.BankAccountId = bankAccountId;
                r.PaymentMethod = paymentMethod;
                r.ReferenceNo = referenceNo;
                r.Description = description;
                r.Status = status;
                r.UpdatedBy = Uid().ToString();
                r.UpdatedDate = DateTime.Now;

                if (r.ReceiptVoucherDetails != null && r.ReceiptVoucherDetails.Count > 0)
                    _context.ReceiptVoucherDetails.RemoveRange(r.ReceiptVoucherDetails);

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

                    _context.ReceiptVoucherDetails.Add(new ReceiptVoucherDetail
                    {
                        ReceiptId = receiptId,
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
                r.Amount = totalDebit;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Receipt updated!" });
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
            var r = await _context.ReceiptVouchers
                .Include(x => x.ReceiptVoucherDetails)
                .FirstOrDefaultAsync(x => x.ReceiptId == id);
            if (r == null)
                return Json(new { success = false, message = "Not found" });

            if (r.ReceiptVoucherDetails != null)
            {
                foreach (var d in r.ReceiptVoucherDetails)
                    _context.ReceiptVoucherDetails.Remove(d);
            }

            r.IsDeleted = true;
            r.DeletedBy = Uid().ToString();
            r.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }

        // -------------------------------------------------------
        // TOGGLE (PENDING/DRAFT -> RECEIVED)
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var r = await _context.ReceiptVouchers.FindAsync(id);
            if (r == null)
                return Json(new { success = false, message = "Not found" });

            var currentUid = Uid().ToString();
            if (r.CreatedBy == currentUid)
                return Json(new { success = false, message = "You cannot approve your own voucher." });

            if (r.Status == "Pending" || r.Status == "Draft")
            {
                r.Status = "Received";
                r.ApprovedBy = Uid().ToString();
                r.ApprovedDate = DateTime.Now;
            }
            else
            {
                r.Status = "Pending";
                r.ApprovedBy = null;
                r.ApprovedDate = null;
            }
            r.UpdatedBy = Uid().ToString();
            r.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Status: " + r.Status });
        }

        // -------------------------------------------------------
        // PROFILE
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var r = await _context.ReceiptVouchers
                .Include(x => x.ReceiptVoucherDetails)
                .ThenInclude(d => d.Account)
                .Include(x => x.Customer)
                .Include(x => x.BankAccount)
                .FirstOrDefaultAsync(x => x.ReceiptId == id);
            if (r == null) return NotFound();

            var c = await _context.Users.FindAsync(r.CreatedBy);
            ViewBag.CreatedByName = c != null ? c.UserName : "N/A";

            // âœ… ApprovedBy/UpdatedBy string hain
            if (!string.IsNullOrWhiteSpace(r.ApprovedBy))
            {
                var a = await _context.Users.FindAsync(r.ApprovedBy);
                ViewBag.ApprovedByName = a != null ? a.UserName : "N/A";
            }
            else
            {
                ViewBag.ApprovedByName = "-";
            }

            if (!string.IsNullOrWhiteSpace(r.UpdatedBy))
            {
                var u = await _context.Users.FindAsync(r.UpdatedBy);
                ViewBag.UpdatedByName = u != null ? u.UserName : "N/A";
            }
            else
            {
                ViewBag.UpdatedByName = "-";
            }

            return View("Profile", r);
        }
    }
}
