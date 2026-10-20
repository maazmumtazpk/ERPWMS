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

namespace ERPWMS.Web.Controllers.HRMaster
{
    [Authorize]
    public class EmployeeBankAccountController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeBankAccountController(AppDbContext c)
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
            var l = await _context.EmployeeBankAccounts.ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(b => b.IsActive == true),
                inactive = l.Count(b => b.IsActive != true),
                addedThisMonth = l.Count(b =>
                    b.CreatedDate.HasValue &&
                    b.CreatedDate.Value.Month == DateTime.Now.Month &&
                    b.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.EmployeeBankAccounts
                .Include(b => b.Employee)
                .OrderBy(b => b.BankAccountId)
                .Select(b => new
                {
                    bankAccountId = b.BankAccountId,
                    employeeName = b.Employee != null
                        ? b.Employee.FirstName + " " + (b.Employee.LastName ?? "")
                        : "N/A",
                    bankName = b.BankName,
                    accountNumber = b.AccountNumber,
                    iban = b.Iban ?? "",
                    accountType = b.AccountType ?? "",
                    isPrimary = b.IsPrimary ?? false,
                    isActive = b.IsActive ?? false
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // GET EMPLOYEES
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var l = await _context.Employees
                .Where(e => e.IsActive == true && e.IsDeleted != true)
                .Select(e => new
                {
                    employeeId = e.EmployeeId,
                    employeeName = e.FirstName + " " + (e.LastName ?? "")
                })
                .ToListAsync();
            return Json(l);
        }

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddBankAccountPartial() => PartialView("_AddBankAccountForm");

        [HttpGet]
        public async Task<IActionResult> EditBankAccountPartial(int id)
        {
            var b = await _context.EmployeeBankAccounts
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.BankAccountId == id);
            if (b == null) return NotFound();
            return PartialView("_EditBankAccountForm", b);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int employeeId,
            string bankName,
            string accountNumber,
            string? iban,
            string? accountType,
            bool isPrimary = false)
        {
            try
            {
                if (employeeId <= 0)
                    return Json(new { success = false, message = "Please select employee." });
                if (string.IsNullOrWhiteSpace(bankName))
                    return Json(new { success = false, message = "Bank name required." });
                if (string.IsNullOrWhiteSpace(accountNumber))
                    return Json(new { success = false, message = "Account number required." });

                var duplicate = await _context.EmployeeBankAccounts
                    .AnyAsync(b => b.EmployeeId == employeeId && b.AccountNumber == accountNumber);
                if (duplicate)
                    return Json(new { success = false, message = "This account number already exists for this employee." });

                if (isPrimary)
                {
                    var existing = await _context.EmployeeBankAccounts
                        .Where(b => b.EmployeeId == employeeId && b.IsPrimary == true)
                        .ToListAsync();
                    foreach (var acc in existing) acc.IsPrimary = false;
                }

                _context.EmployeeBankAccounts.Add(new EmployeeBankAccount
                {
                    EmployeeId = employeeId,
                    BankName = bankName ?? "",
                    AccountNumber = accountNumber ?? "",
                    Iban = iban,
                    AccountType = accountType,
                    IsPrimary = isPrimary,
                    IsActive = true,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Bank account added!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // -------------------------------------------------------
        // EDIT
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int bankAccountId,
            string bankName,
            string accountNumber,
            string? iban,
            string? accountType,
            bool isPrimary = false,
            bool isActive = true)
        {
            try
            {
                var b = await _context.EmployeeBankAccounts.FindAsync(bankAccountId);
                if (b == null)
                    return Json(new { success = false, message = "Not found" });

                var duplicate = await _context.EmployeeBankAccounts
                    .AnyAsync(x => x.EmployeeId == b.EmployeeId && x.AccountNumber == accountNumber && x.BankAccountId != bankAccountId);
                if (duplicate)
                    return Json(new { success = false, message = "This account number already exists for this employee." });

                b.BankName = bankName;
                b.AccountNumber = accountNumber;
                b.Iban = iban;
                b.AccountType = accountType;
                b.IsPrimary = isPrimary;
                b.IsActive = isActive;
                b.UpdatedBy = Uid().ToString();
                b.UpdatedDate = DateTime.Now;

                if (isPrimary)
                {
                    var others = await _context.EmployeeBankAccounts
                        .Where(x => x.EmployeeId == b.EmployeeId && x.BankAccountId != bankAccountId && x.IsPrimary == true)
                        .ToListAsync();
                    foreach (var acc in others) acc.IsPrimary = false;
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
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
                var b = await _context.EmployeeBankAccounts.FindAsync(id);
                if (b == null)
                    return Json(new { success = false, message = "Not found" });

                _context.EmployeeBankAccounts.Remove(b);
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
                var b = await _context.EmployeeBankAccounts.FindAsync(id);
                if (b == null)
                    return Json(new { success = false, message = "Not found" });

                b.IsActive = !(b.IsActive ?? false);
                b.UpdatedBy = Uid().ToString();
                b.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = b.IsActive == true ? "Activated." : "Deactivated." });
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
            try
            {
                var b = await _context.EmployeeBankAccounts
                    .Include(x => x.Employee)
                    .FirstOrDefaultAsync(x => x.BankAccountId == id);

                if (b == null) return NotFound();

                var creator = await _context.Users.FindAsync(b.CreatedBy);


                // âœ… UpdatedBy string hai, direct use karo
                if (!string.IsNullOrWhiteSpace(b.UpdatedBy))
                {
                    var upd = await _context.Users.FindAsync(b.UpdatedBy);

                }
                else
                {

                }

                return View("Profile", b);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
                return RedirectToAction("Index");
            }
        }
    }
}
