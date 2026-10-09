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
    public class ChartOfAccountController : Controller
    {
        private readonly AppDbContext _context;

        public ChartOfAccountController(AppDbContext c)
        {
            _context = c;
        }

        private int Uid() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 1;

        // -------------------------------------------------------
        // INDEX
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult Index() => View();

        // -------------------------------------------------------
        // STATISTICS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.ChartOfAccounts.Where(a => a.IsDeleted != true).ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(a => a.IsActive == true),
                inactive = l.Count(a => a.IsActive != true),
                addedThisMonth = l.Count(a =>
                    a.CreatedDate.HasValue &&
                    a.CreatedDate.Value.Month == DateTime.Now.Month &&
                    a.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.ChartOfAccounts
                .Where(a => a.IsDeleted != true)
                .OrderBy(a => a.AccountId)
                .Select(a => new
                {
                    accountId = a.AccountId,
                    accountCode = a.AccountCode,
                    accountName = a.AccountName,
                    accountType = a.AccountType,
                    isActive = a.IsActive ?? false,
                    createdDate = a.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddAccountPartial() => PartialView("_AddAccountForm");

        [HttpGet]
        public async Task<IActionResult> EditAccountPartial(int id)
        {
            var a = await _context.ChartOfAccounts.FindAsync(id);
            if (a == null) return NotFound();
            return PartialView("_EditAccountForm", a);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string accountCode, string accountName, string accountType)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accountName))
                    return Json(new { success = false, message = "Account name required." });
                if (string.IsNullOrWhiteSpace(accountCode))
                    return Json(new { success = false, message = "Account code required." });

                var d = await _context.ChartOfAccounts.AnyAsync(a => a.AccountCode == accountCode && a.IsDeleted != true);
                if (d)
                    return Json(new { success = false, message = "Account code already exists." });

                _context.ChartOfAccounts.Add(new ChartOfAccount
                {
                    AccountCode = accountCode.Trim(),
                    AccountName = accountName.Trim(),
                    AccountType = accountType ?? "Asset",
                    CompanyId = 1,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Account created!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // EDIT
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int accountId, string accountCode, string accountName, string accountType, bool isActive = true)
        {
            try
            {
                var a = await _context.ChartOfAccounts.FindAsync(accountId);
                if (a == null)
                    return Json(new { success = false, message = "Not found" });

                a.AccountCode = accountCode.Trim();
                a.AccountName = accountName.Trim();
                a.AccountType = accountType;
                a.IsActive = isActive;
                a.UpdatedBy = Uid().ToString();
                a.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Account updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // DELETE
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var a = await _context.ChartOfAccounts.FindAsync(id);
                if (a == null)
                    return Json(new { success = false, message = "Not found" });

                a.IsDeleted = true;
                a.IsActive = false;
                a.DeletedBy = Uid().ToString();
                a.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // TOGGLE ACTIVE
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var a = await _context.ChartOfAccounts.FindAsync(id);
                if (a == null)
                    return Json(new { success = false, message = "Not found" });

                a.IsActive = !(a.IsActive ?? false);
                a.UpdatedBy = Uid().ToString();
                a.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = a.IsActive == true ? "Activated." : "Deactivated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // PROFILE
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var a = await _context.ChartOfAccounts.FindAsync(id);
            if (a == null) return NotFound();

            // Account Ledger Entries laao
            try
            {
                var entries = await _context.AccountLedgers
                    .Where(l => l.AccountId == id)
                    .OrderByDescending(l => l.EntryDate)
                    .Take(100)
                    .ToListAsync();

                ViewBag.LedgerEntries = entries;

                // Totals calculate karo — Debit/Credit decimal? hain
                decimal totalDebit = entries
                    .Where(e => e.Debit.HasValue)
                    .Sum(e => e.Debit!.Value);

                decimal totalCredit = entries
                    .Where(e => e.Credit.HasValue)
                    .Sum(e => e.Credit!.Value);

                ViewBag.TotalDebit = totalDebit;
                ViewBag.TotalCredit = totalCredit;
                ViewBag.CurrentBalance = entries.Count > 0 ? entries[0].Balance : 0m;
            }
            catch
            {
                ViewBag.LedgerEntries = new List<AccountLedger>();
                ViewBag.TotalDebit = 0m;
                ViewBag.TotalCredit = 0m;
                ViewBag.CurrentBalance = 0m;
            }

            // Company Name
            try
            {
                var company = await _context.Companies.FindAsync(a.CompanyId);
                ViewBag.CompanyName = company != null ? company.CompanyName : "N/A";
            }
            catch { ViewBag.CompanyName = "N/A"; }

            // Parent Account Name
            if (a.ParentAccountId.HasValue)
            {
                var parent = await _context.ChartOfAccounts.FindAsync(a.ParentAccountId.Value);
                ViewBag.ParentAccountName = parent != null ? parent.AccountName : "N/A";
            }
            else
            {
                ViewBag.ParentAccountName = "None (Main Account)";
            }

            return View("Profile", a);
        }
    }
}