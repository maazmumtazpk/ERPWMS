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
    public class JournalVoucherController : Controller
    {
        private readonly AppDbContext _context;

        public JournalVoucherController(AppDbContext c)
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
            var l = await _context.JournalVouchers.Where(j => j.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(j => j.Status == "Posted" || j.Status == "Approved"),
                inactive = l.Count(j => j.Status == "Draft" || j.Status == "Pending"),
                addedThisMonth = l.Count(j =>
                    j.CreatedDate.HasValue &&
                    j.CreatedDate.Value.Month == DateTime.Now.Month &&
                    j.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.JournalVouchers
                .Where(j => j.IsDeleted != true)
                .OrderBy(j => j.VoucherId)
                .Select(j => new
                {
                    voucherId = j.VoucherId,
                    voucherNo = j.VoucherNo,
                    voucherDate = j.VoucherDate,
                    description = j.Description,
                    totalDebit = j.TotalDebit,
                    totalCredit = j.TotalCredit,
                    status = j.Status ?? "Draft",
                    createdDate = j.CreatedDate
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
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddJournalPartial() => PartialView("_AddJournalForm");

        [HttpGet]
        public async Task<IActionResult> EditJournalPartial(int id)
        {
            var j = await _context.JournalVouchers
                .Include(x => x.JournalVoucherDetails)
                .FirstOrDefaultAsync(x => x.VoucherId == id);
            if (j == null) return NotFound();
            return PartialView("_EditJournalForm", j);
        }

        // -------------------------------------------------------
        // NEXT VOUCHER NO
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetNextVoucherNo()
        {
            return Json(new { voucherNo = await GenerateNextVoucherNo() });
        }

        private async Task<string> GenerateNextVoucherNo()
        {
            try
            {
                var year = DateTime.Now.Year;
                var prefix = "JV-" + year + "-";
                var existing = await _context.JournalVouchers
                    .Where(j => j.VoucherNo != null && j.VoucherNo.StartsWith(prefix))
                    .Select(j => j.VoucherNo)
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
                return "JV-" + DateTime.Now.Year + "-" + DateTime.Now.ToString("HHmmss");
            }
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string voucherNo, DateTime? voucherDate, string? description)
        {
            try
            {
                // Auto-generate voucher no if empty
                if (string.IsNullOrWhiteSpace(voucherNo))
                    voucherNo = await GenerateNextVoucherNo();
                else if (await _context.JournalVouchers.AnyAsync(j => j.VoucherNo == voucherNo && j.IsDeleted != true))
                    voucherNo = await GenerateNextVoucherNo();

                var f = Request.Form;
                var details = new List<JournalVoucherDetail>();
                decimal totalDebit = 0, totalCredit = 0;

                // Naye pattern se entries nikaalo
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

                    details.Add(new JournalVoucherDetail
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
                    return Json(new { success = false, message = "Debit (" + totalDebit + ") and Credit (" + totalCredit + ") must be equal." });

                var voucher = new JournalVoucher
                {
                    VoucherNo = voucherNo.Trim(),
                    VoucherDate = voucherDate ?? DateTime.Now,
                    Description = description ?? "",
                    TotalDebit = totalDebit,
                    TotalCredit = totalCredit,
                    Status = "Draft",
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                };
                _context.JournalVouchers.Add(voucher);
                await _context.SaveChangesAsync();

                foreach (var dt in details)
                {
                    dt.VoucherId = voucher.VoucherId;
                    _context.JournalVoucherDetails.Add(dt);
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Journal voucher created: " + voucher.VoucherNo });
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
        public async Task<IActionResult> Edit(int voucherId, string voucherNo, DateTime? voucherDate, string? description, string? status)
        {
            try
            {
                var j = await _context.JournalVouchers
                    .Include(x => x.JournalVoucherDetails)
                    .FirstOrDefaultAsync(x => x.VoucherId == voucherId);
                if (j == null)
                    return Json(new { success = false, message = "Not found" });

                j.VoucherNo = voucherNo.Trim();
                j.VoucherDate = voucherDate ?? j.VoucherDate;
                j.Description = description;
                j.Status = status;
                j.UpdatedBy = Uid().ToString();
                j.UpdatedDate = DateTime.Now;

                if (j.JournalVoucherDetails != null && j.JournalVoucherDetails.Count > 0)
                    _context.JournalVoucherDetails.RemoveRange(j.JournalVoucherDetails);

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

                    _context.JournalVoucherDetails.Add(new JournalVoucherDetail
                    {
                        VoucherId = voucherId,
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
                j.TotalDebit = totalDebit;
                j.TotalCredit = totalCredit;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Journal updated!" });
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
            var j = await _context.JournalVouchers
                .Include(x => x.JournalVoucherDetails)
                .FirstOrDefaultAsync(x => x.VoucherId == id);
            if (j == null)
                return Json(new { success = false, message = "Not found" });

            if (j.JournalVoucherDetails != null)
            {
                foreach (var d in j.JournalVoucherDetails)
                    _context.JournalVoucherDetails.Remove(d);
            }

            j.IsDeleted = true;
            j.DeletedBy = Uid().ToString();
            j.DeletedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }

        // -------------------------------------------------------
        // TOGGLE (POST / UNPOST)
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var j = await _context.JournalVouchers.FindAsync(id);
                if (j == null)
                    return Json(new { success = false, message = "Not found" });

                var currentUid = Uid().ToString();
                if (j.CreatedBy == currentUid)
                    return Json(new { success = false, message = "You cannot approve your own voucher." });

                if (j.Status == "Draft" || string.IsNullOrEmpty(j.Status))
                {
                    j.Status = "Posted";
                    j.ApprovedBy = Uid().ToString();
                    j.ApprovedDate = DateTime.Now;
                }
                else
                {
                    j.Status = "Draft";
                    j.ApprovedBy = null;
                    j.ApprovedDate = null;
                }

                j.UpdatedBy = Uid().ToString();
                j.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + j.Status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // -------------------------------------------------------
        // PROFILE
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var j = await _context.JournalVouchers
                .Include(x => x.JournalVoucherDetails)
                .ThenInclude(d => d.Account)
                .FirstOrDefaultAsync(x => x.VoucherId == id);
            if (j == null) return NotFound();

            // âœ… ApprovedBy/UpdatedBy string hain, direct use karo
            if (!string.IsNullOrWhiteSpace(j.ApprovedBy))
            {
                var a = await _context.Users.FindAsync(j.ApprovedBy);
                ViewBag.ApprovedByName = a != null ? a.UserName : "N/A";
            }
            else
            {
                ViewBag.ApprovedByName = "-";
            }

            var c = await _context.Users.FindAsync(j.CreatedBy);
            ViewBag.CreatedByName = c != null ? c.UserName : "N/A";

            if (!string.IsNullOrWhiteSpace(j.UpdatedBy))
            {
                var u = await _context.Users.FindAsync(j.UpdatedBy);
                ViewBag.UpdatedByName = u != null ? u.UserName : "N/A";
            }
            else
            {
                ViewBag.UpdatedByName = "-";
            }

            return View("Profile", j);
        }
    }
}
