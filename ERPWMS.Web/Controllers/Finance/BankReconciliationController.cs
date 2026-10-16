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
    public class BankReconciliationController : Controller
    {
        private readonly AppDbContext _context;

        public BankReconciliationController(AppDbContext c)
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
            var l = await _context.BankReconciliations.ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(r => r.Status == "Reconciled"),
                inactive = l.Count(r => r.Status == "Pending"),
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
            var l = await _context.BankReconciliations
                .Include(r => r.BankAccount)
                .OrderBy(r => r.ReconciliationId)
                .Select(r => new
                {
                    reconciliationId = r.ReconciliationId,
                    bankAccount = r.BankAccount != null ? r.BankAccount.AccountTitle : "N/A",
                    statementDate = r.StatementDate,
                    statementBalance = r.StatementBalance,
                    systemBalance = r.SystemBalance,
                    difference = r.Difference,
                    status = r.Status ?? "Pending",
                    reconciliationDate = r.ReconciliationDate
                })
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
                    accountTitle = b.AccountTitle
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddReconciliationPartial() => PartialView("_AddReconciliationForm");

        [HttpGet]
        public async Task<IActionResult> EditReconciliationPartial(int id)
        {
            var r = await _context.BankReconciliations.FindAsync(id);
            if (r == null) return NotFound();
            return PartialView("_EditReconciliationForm", r);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int bankAccountId,
            DateOnly statementDate,
            decimal statementBalance,
            decimal systemBalance,
            string? remarks)
        {
            try
            {
                _context.BankReconciliations.Add(new BankReconciliation
                {
                    BankAccountId = bankAccountId,
                    StatementDate = statementDate,
                    StatementBalance = statementBalance,
                    SystemBalance = systemBalance,
                    Difference = statementBalance - systemBalance,
                    Status = "Pending",
                    ReconciliationDate = DateTime.Now,
                    Remarks = remarks ?? "",
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Reconciliation created!" });
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
            int reconciliationId,
            decimal statementBalance,
            decimal systemBalance,
            string? status,
            string? remarks)
        {
            try
            {
                var r = await _context.BankReconciliations.FindAsync(reconciliationId);
                if (r == null)
                    return Json(new { success = false, message = "Not found" });

                r.StatementBalance = statementBalance;
                r.SystemBalance = systemBalance;
                r.Difference = statementBalance - systemBalance;
                r.Status = status;
                r.Remarks = remarks;
                r.UpdatedBy = Uid().ToString();
                r.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Reconciliation updated!" });
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
            try
            {
                var r = await _context.BankReconciliations.FindAsync(id);
                if (r == null)
                    return Json(new { success = false, message = "Not found" });

                _context.BankReconciliations.Remove(r);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        // -------------------------------------------------------
        // TOGGLE STATUS
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var r = await _context.BankReconciliations.FindAsync(id);
                if (r == null)
                    return Json(new { success = false, message = "Not found" });

                r.Status = r.Status == "Pending" ? "Reconciled" : "Pending";
                r.UpdatedBy = Uid().ToString();
                r.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + r.Status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Operation failed." });
            }
        }
    }
}
