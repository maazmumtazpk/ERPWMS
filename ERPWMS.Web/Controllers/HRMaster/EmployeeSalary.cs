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
    public class EmployeeSalaryController : Controller
    {
        private readonly AppDbContext _context;

        public EmployeeSalaryController(AppDbContext c)
        {
            _context = c;
        }

        private int Uid() => int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : 1;

        [HttpGet]
        public IActionResult Index() => View();

        // -------------------------------------------------------
        // STATISTICS
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var l = await _context.EmployeeSalaries.ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(s => s.IsActive == true),
                inactive = l.Count(s => s.IsActive != true),
                addedThisMonth = l.Count(s =>
                    s.CreatedDate.HasValue &&
                    s.CreatedDate.Value.Month == DateTime.Now.Month &&
                    s.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // -------------------------------------------------------
        // GET ALL
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.EmployeeSalaries
                .Include(s => s.Employee)
                .Where(s => s.IsActive != false)
                .OrderBy(s => s.SalaryId)
                .Select(s => new
                {
                    salaryId = s.SalaryId,
                    employeeName = s.Employee != null
                        ? s.Employee.FirstName + " " + (s.Employee.LastName ?? "")
                        : "N/A",
                    basicSalary = s.BasicSalary,
                    totalAllowances = s.TotalAllowances,
                    totalDeductions = s.TotalDeductions,
                    netSalary = s.NetSalary,
                    effectiveDate = s.EffectiveDate,
                    isActive = s.IsActive ?? false
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
        public IActionResult AddSalaryPartial() => PartialView("_AddSalaryForm");

        [HttpGet]
        public async Task<IActionResult> EditSalaryPartial(int id)
        {
            var s = await _context.EmployeeSalaries.FindAsync(id);
            if (s == null) return NotFound();
            return PartialView("_EditSalaryForm", s);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int employeeId,
            DateOnly effectiveDate,
            decimal basicSalary,
            decimal? houseRent,
            decimal? utilityAllowance,
            decimal? medicalAllowance,
            decimal? otherAllowances,
            decimal? incomeTax,
            decimal? professionalTax,
            decimal? otherDeductions)
        {
            try
            {
                if (employeeId <= 0)
                    return Json(new { success = false, message = "Please select employee." });

                var duplicate = await _context.EmployeeSalaries
                    .AnyAsync(s => s.EmployeeId == employeeId && s.EffectiveDate == effectiveDate && s.IsActive != false);
                if (duplicate)
                    return Json(new { success = false, message = "Salary already exists for this employee and date." });

                decimal ta = (houseRent ?? 0) + (utilityAllowance ?? 0) + (medicalAllowance ?? 0) + (otherAllowances ?? 0);
                decimal td = (incomeTax ?? 0) + (professionalTax ?? 0) + (otherDeductions ?? 0);
                decimal net = basicSalary + ta - td;

                _context.EmployeeSalaries.Add(new EmployeeSalary
                {
                    EmployeeId = employeeId,
                    EffectiveDate = effectiveDate,
                    BasicSalary = basicSalary,
                    HouseRent = houseRent,
                    UtilityAllowance = utilityAllowance,
                    MedicalAllowance = medicalAllowance,
                    OtherAllowances = otherAllowances,
                    TotalAllowances = ta,          // âœ… decimal? (string nahi)
                    IncomeTax = incomeTax,
                    ProfessionalTax = professionalTax,
                    OtherDeductions = otherDeductions,
                    TotalDeductions = td,          // âœ… decimal?
                    NetSalary = net,               // âœ… decimal?
                    IsActive = true,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Salary added!" });
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
            int salaryId,
            decimal basicSalary,
            decimal? houseRent,
            decimal? utilityAllowance,
            decimal? medicalAllowance,
            decimal? otherAllowances,
            decimal? incomeTax,
            decimal? professionalTax,
            decimal? otherDeductions,
            bool isActive = true)
        {
            try
            {
                var s = await _context.EmployeeSalaries.FindAsync(salaryId);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                s.BasicSalary = basicSalary;
                s.HouseRent = houseRent;
                s.UtilityAllowance = utilityAllowance;
                s.MedicalAllowance = medicalAllowance;
                s.OtherAllowances = otherAllowances;

                decimal ta = (houseRent ?? 0) + (utilityAllowance ?? 0) + (medicalAllowance ?? 0) + (otherAllowances ?? 0);
                decimal td = (incomeTax ?? 0) + (professionalTax ?? 0) + (otherDeductions ?? 0);

                s.TotalAllowances = ta;            // âœ… decimal?
                s.IncomeTax = incomeTax;
                s.ProfessionalTax = professionalTax;
                s.OtherDeductions = otherDeductions;
                s.TotalDeductions = td;            // âœ… decimal?
                s.NetSalary = basicSalary + ta - td; // âœ… decimal?

                s.IsActive = isActive;
                s.UpdatedBy = Uid().ToString();
                s.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // -------------------------------------------------------
        // DELETE (soft delete)
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var s = await _context.EmployeeSalaries.FindAsync(id);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                s.IsActive = false;
                s.UpdatedBy = Uid().ToString();
                s.UpdatedDate = DateTime.Now;
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
                var s = await _context.EmployeeSalaries.FindAsync(id);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                s.IsActive = !(s.IsActive ?? false);
                s.UpdatedBy = Uid().ToString();
                s.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = s.IsActive == true ? "Activated." : "Deactivated." });
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
                var s = await _context.EmployeeSalaries
                    .Include(x => x.Employee)
                    .FirstOrDefaultAsync(x => x.SalaryId == id);

                if (s == null) return NotFound();

                var creator = await _context.Users.FindAsync(s.CreatedBy);


                // âœ… UpdatedBy string hai
                if (!string.IsNullOrWhiteSpace(s.UpdatedBy))
                {
                    var upd = await _context.Users.FindAsync(s.UpdatedBy);

                }
                else
                {

                }

                return View("Profile", s);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
                return RedirectToAction("Index");
            }
        }
    }
}
