using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers.Finance
{
    [Authorize]
    public class BankAccountController : Controller
    {
        private readonly AppDbContext _context;

        public BankAccountController(AppDbContext c)
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

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.BankAccounts.Where(b => b.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(b => b.IsActive == true),
                inactive = l.Count(b => b.IsActive != true),
                addedThisMonth = l.Count(b => b.CreatedDate.HasValue && b.CreatedDate.Value.Month == DateTime.Now.Month && b.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.BankAccounts
                .Where(b => b.IsDeleted != true)
                .OrderBy(b => b.BankAccountId)
                .Select(b => new
                {
                    bankAccountId = b.BankAccountId,
                    accountTitle = b.AccountTitle,
                    accountNo = b.AccountNo,
                    bankName = b.BankName,
                    iban = b.Iban ?? "",
                    openingBalance = b.OpeningBalance,
                    isActive = b.IsActive ?? false,
                    createdDate = b.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var l = await _context.Companies
                .Where(c => c.IsDeleted != true)
                .Select(c => new { companyId = c.CompanyId, companyName = c.CompanyName })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public IActionResult AddBankPartial() => PartialView("_AddBankForm");

        [HttpGet]
        public async Task<IActionResult> EditBankPartial(int id)
        {
            var b = await _context.BankAccounts.FindAsync(id);
            if (b == null) return NotFound();
            return PartialView("_EditBankForm", b);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int companyId, string accountTitle, string accountNo, string bankName, string iban, decimal openingBalance)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountTitle))
                    return Json(new { success = false, message = "Account title required." });
                if (string.IsNullOrWhiteSpace(accountNo))
                    return Json(new { success = false, message = "Account number required." });
                if (companyId <= 0) companyId = 1;

                _context.BankAccounts.Add(new BankAccount
                {
                    CompanyId = companyId,
                    AccountTitle = accountTitle.Trim(),
                    AccountNo = accountNo.Trim(),
                    BankName = bankName ?? "",
                    Iban = iban,
                    OpeningBalance = openingBalance,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Bank account created!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int bankAccountId, string accountTitle, string accountNo, string bankName, string iban, decimal openingBalance, bool isActive = true)
        {
            try
            {
                var b = await _context.BankAccounts.FindAsync(bankAccountId);
                if (b == null) return Json(new { success = false, message = "Not found" });

                b.AccountTitle = accountTitle.Trim();
                b.AccountNo = accountNo.Trim();
                b.BankName = bankName;
                b.Iban = iban;
                b.OpeningBalance = openingBalance;
                b.IsActive = isActive;
                b.UpdatedBy = Uid().ToString();
                b.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Bank account updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var b = await _context.BankAccounts.FindAsync(id);
                if (b == null) return Json(new { success = false, message = "Not found" });

                b.IsDeleted = true;
                b.IsActive = false;
                b.DeletedBy = Uid().ToString();
                b.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var b = await _context.BankAccounts.FindAsync(id);
                if (b == null) return Json(new { success = false, message = "Not found" });

                b.IsActive = !(b.IsActive ?? false);
                b.UpdatedBy = Uid().ToString();
                b.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = b.IsActive == true ? "Activated." : "Deactivated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var b = await _context.BankAccounts.FindAsync(id);
            if (b == null) return NotFound();

            // Company Name
            var comp = await _context.Companies.FindAsync(b.CompanyId);
            ViewBag.CompanyName = comp != null ? comp.CompanyName : "N/A";

            // Creator Name
            var creator = await _context.Users.FindAsync(b.CreatedBy);


            // âœ… UpdatedBy string hai, direct use karo
            if (!string.IsNullOrWhiteSpace(b.UpdatedBy))
            {
                var upd = await _context.Users.FindAsync(b.UpdatedBy);

            }
            else
            {

            }

            // Transaction History
            try
            {
                var transactions = await _context.BankTransactions
                    .Where(t => t.BankAccountId == id)
                    .OrderByDescending(t => t.TransactionDate)
                    .Take(50)
                    .ToListAsync();
                ViewBag.Transactions = transactions;

                decimal totalIn = transactions.Where(t => t.TransactionType == "Credit" || t.TransactionType == "Deposit").Sum(t => t.Amount);
                decimal totalOut = transactions.Where(t => t.TransactionType == "Debit" || t.TransactionType == "Withdrawal").Sum(t => t.Amount);
                ViewBag.TotalIn = totalIn;
                ViewBag.TotalOut = totalOut;
                ViewBag.CurrentBalance = (b.OpeningBalance + totalIn - totalOut);
            }
            catch
            {
                ViewBag.Transactions = new List<ERPWMS.Database.Models.BankTransaction>();
                ViewBag.TotalIn = 0m;
                ViewBag.TotalOut = 0m;
                ViewBag.CurrentBalance = b.OpeningBalance;
            }

            return View("Profile", b);
        }
    }
}

