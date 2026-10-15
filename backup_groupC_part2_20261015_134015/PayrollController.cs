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

namespace ERPWMS.Web.Controllers.HRTransaction
{
    [Authorize]
    public class PayrollController : Controller
    {
        private readonly AppDbContext _context;

        public PayrollController(AppDbContext c)
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
            var l = await _context.EmployeePayrolls.ToListAsync();
            return Json(new
            {
                total = l.Count,
                active = l.Count(p => p.Status == "Paid" || p.Status == "Approved"),
                inactive = l.Count(p => p.Status == "Draft" || p.Status == "Submitted"),
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
            var l = await _context.EmployeePayrolls
                .Include(p => p.Employee)
                .OrderBy(p => p.PayrollId)
                .Select(p => new
                {
                    payrollId = p.PayrollId,
                    employeeName = p.Employee != null
                        ? p.Employee.FirstName + " " + (p.Employee.LastName ?? "")
                        : "N/A",
                    monthYear = p.MonthYear,
                    grossPay = p.GrossPay,
                    totalDeductions = p.TotalDeductions,    // ✅ decimal?
                    netPay = p.NetPay,
                    status = p.Status ?? "Draft",
                    paymentDate = p.PaymentDate
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
        // GET EMPLOYEE SALARY
        // -------------------------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetEmployeeSalary(int employeeId)
        {
            try
            {
                if (employeeId <= 0)
                    return Json(new { success = false, grossPay = 0 });

                var salary = await _context.EmployeeSalaries
                    .Where(s => s.EmployeeId == employeeId && s.IsActive != false)
                    .OrderByDescending(s => s.EffectiveDate)
                    .FirstOrDefaultAsync();

                if (salary == null)
                {
                    var emp = await _context.Employees.FindAsync(employeeId);
                    decimal baseSal = emp != null ? emp.BaseSalary : 0;
                    return Json(new { success = true, grossPay = baseSal, netPay = baseSal, totalDeductions = 0, salaryId = 0 });
                }

                // ✅ Debit/Credit decimal? hain
                decimal gross = salary.BasicSalary
                    + (salary.HouseRent ?? 0)
                    + (salary.UtilityAllowance ?? 0)
                    + (salary.MedicalAllowance ?? 0)
                    + (salary.OtherAllowances ?? 0);

                decimal deductions = (salary.IncomeTax ?? 0)
                    + (salary.ProfessionalTax ?? 0)
                    + (salary.OtherDeductions ?? 0);

                decimal net = gross - deductions;

                return Json(new
                {
                    success = true,
                    salaryId = salary.SalaryId,
                    grossPay = gross,
                    totalDeductions = deductions,
                    netPay = net
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message, grossPay = 0 });
            }
        }

        // -------------------------------------------------------
        // ADD / EDIT PARTIALS
        // -------------------------------------------------------
        [HttpGet]
        public IActionResult AddPayrollPartial() => PartialView("_AddPayrollForm");

        [HttpGet]
        public async Task<IActionResult> EditPayrollPartial(int id)
        {
            var p = await _context.EmployeePayrolls
                .Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.PayrollId == id);
            if (p == null) return NotFound();
            return PartialView("_EditPayrollForm", p);
        }

        // -------------------------------------------------------
        // REGISTER
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int employeeId,
            DateOnly monthYear,
            decimal grossPay,
            decimal? totalDeductions,
            decimal netPay)
        {
            try
            {
                if (employeeId <= 0)
                    return Json(new { success = false, message = "Please select employee." });

                var existing = await _context.EmployeePayrolls
                    .FirstOrDefaultAsync(p => p.EmployeeId == employeeId
                        && p.MonthYear.Year == monthYear.Year
                        && p.MonthYear.Month == monthYear.Month);
                if (existing != null)
                    return Json(new { success = false, message = "Payroll already exists for this employee and month." });

                var salary = await _context.EmployeeSalaries
                    .Where(s => s.EmployeeId == employeeId && s.IsActive != false)
                    .OrderByDescending(s => s.EffectiveDate)
                    .FirstOrDefaultAsync();

                int salaryId = salary != null ? salary.SalaryId : 0;

                if (salaryId == 0)
                {
                    var anySal = await _context.EmployeeSalaries.FirstOrDefaultAsync();
                    if (anySal == null)
                        return Json(new { success = false, message = "No salary record found. Please add salary first." });
                    salaryId = anySal.SalaryId;
                }

                _context.EmployeePayrolls.Add(new EmployeePayroll
                {
                    EmployeeId = employeeId,
                    SalaryId = salaryId,
                    MonthYear = monthYear,
                    GrossPay = grossPay,
                    TotalDeductions = totalDeductions ?? 0,   // ✅ decimal?
                    NetPay = netPay,
                    Status = "Draft",
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Payroll created!" });
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
            int payrollId,
            decimal grossPay,
            decimal? totalDeductions,
            decimal netPay,
            string? status)
        {
            try
            {
                var p = await _context.EmployeePayrolls.FindAsync(payrollId);
                if (p == null)
                    return Json(new { success = false, message = "Not found" });

                p.GrossPay = grossPay;
                p.TotalDeductions = totalDeductions;    // ✅ decimal?
                p.NetPay = netPay;
                p.Status = status;
                if (status == "Paid") p.PaymentDate = DateTime.Now;
                p.UpdatedBy = Uid().ToString();
                p.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Payroll updated!" });
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
                var p = await _context.EmployeePayrolls.FindAsync(id);
                if (p == null)
                    return Json(new { success = false, message = "Not found" });

                _context.EmployeePayrolls.Remove(p);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // -------------------------------------------------------
        // TOGGLE STATUS
        // Draft -> Submitted -> Approved -> Paid -> Draft
        // -------------------------------------------------------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id)
        {
            try
            {
                var p = await _context.EmployeePayrolls.FindAsync(id);
                if (p == null)
                    return Json(new { success = false, message = "Not found" });

                if (p.Status == "Draft")
                {
                    p.Status = "Submitted";
                }
                else if (p.Status == "Submitted")
                {
                    p.Status = "Approved";
                    p.ApprovedBy = Uid().ToString();
                    p.ApprovedDate = DateTime.Now;
                }
                else if (p.Status == "Approved")
                {
                    p.Status = "Paid";
                    p.PaymentDate = DateTime.Now;
                }
                else
                {
                    p.Status = "Draft";
                    p.ApprovedBy = null;
                    p.ApprovedDate = null;
                    p.PaymentDate = null;
                }

                p.UpdatedBy = Uid().ToString();
                p.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Status: " + p.Status });
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
                var p = await _context.EmployeePayrolls
                    .Include(x => x.Employee)
                    .FirstOrDefaultAsync(x => x.PayrollId == id);

                if (p == null) return NotFound();

                var creator = await _context.Users.FindAsync(p.CreatedBy);
                ViewBag.CreatedByName = creator != null ? creator.UserName : "N/A";

                // ✅ ApprovedBy string hai
                if (!string.IsNullOrWhiteSpace(p.ApprovedBy))
                {
                    var approver = await _context.Users.FindAsync(p.ApprovedBy);
                    ViewBag.ApprovedByName = approver != null ? approver.UserName : "N/A";
                }
                else
                {
                    ViewBag.ApprovedByName = "-";
                }

                // ✅ UpdatedBy string hai
                if (!string.IsNullOrWhiteSpace(p.UpdatedBy))
                {
                    var upd = await _context.Users.FindAsync(p.UpdatedBy);
                    ViewBag.UpdatedByName = upd != null ? upd.UserName : "N/A";
                }
                else
                {
                    ViewBag.UpdatedByName = "-";
                }

                return View("Profile", p);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
                return RedirectToAction("Index");
            }
        }
    }
}