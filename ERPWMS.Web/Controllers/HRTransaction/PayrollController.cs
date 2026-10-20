using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers.HRTransaction
{
    [Authorize(Policy = "Payroll.Access")]
    public class PayrollController : Controller
    {
        private readonly AppDbContext _context;
        public PayrollController(AppDbContext c) { _context = c; }

        private int UserId
        {
            get
            {
                var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(cl, out int id) || id <= 0)
                    throw new UnauthorizedAccessException("User identity missing.");
                return id;
            }
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            var now = DateTime.Now;
            var q = _context.EmployeePayrolls;
            return Json(new
            {
                total = await q.CountAsync(),
                active = await q.CountAsync(p => p.Status == "Paid" || p.Status == "Approved"),
                inactive = await q.CountAsync(p => p.Status == "Draft" || p.Status == "Submitted"),
                addedThisMonth = await q.CountAsync(p =>
                    p.CreatedDate.HasValue &&
                    p.CreatedDate.Value.Month == now.Month &&
                    p.CreatedDate.Value.Year == now.Year)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.EmployeePayrolls.Include(p => p.Employee)
                .OrderBy(p => p.PayrollId)
                .Select(p => new
                {
                    payrollId = p.PayrollId,
                    employeeName = p.Employee != null
                        ? p.Employee.FirstName + " " + (p.Employee.LastName ?? "")
                        : "N/A",
                    monthYear = p.MonthYear,
                    grossPay = p.GrossPay,
                    totalDeductions = p.TotalDeductions,
                    netPay = p.NetPay,
                    status = p.Status ?? "Draft",
                    paymentDate = p.PaymentDate
                }).ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees() =>
            Json(await _context.Employees
                .Where(e => e.IsActive == true && e.IsDeleted != true)
                .Select(e => new { employeeId = e.EmployeeId, employeeName = e.FirstName + " " + (e.LastName ?? "") })
                .ToListAsync());

        [HttpGet]
        public async Task<IActionResult> GetEmployeeSalary(int employeeId)
        {
            if (employeeId <= 0)
                return Json(new { success = false, grossPay = 0 });

            var salary = await _context.EmployeeSalaries
                .Where(s => s.EmployeeId == employeeId && s.IsActive == true)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefaultAsync();

            if (salary == null)
            {
                var emp = await _context.Employees.FindAsync(employeeId);
                decimal baseSal = emp?.BaseSalary ?? 0;
                return Json(new { success = true, grossPay = baseSal, netPay = baseSal, totalDeductions = 0m, salaryId = 0 });
            }

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

        [HttpGet]
        public IActionResult AddPayrollPartial() => PartialView("_AddPayrollForm");

        [HttpGet]
        public async Task<IActionResult> EditPayrollPartial(int id)
        {
            var p = await _context.EmployeePayrolls.Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.PayrollId == id);
            if (p == null) return NotFound();
            return PartialView("_EditPayrollForm", p);
        }

        // ---------- REGISTER: server calculates amounts ----------
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int employeeId, DateOnly monthYear)
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
                .Where(s => s.EmployeeId == employeeId && s.IsActive == true)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefaultAsync();
            if (salary == null)
                return Json(new { success = false, message = "No active salary record found for employee." });

            decimal gross = salary.BasicSalary
                + (salary.HouseRent ?? 0)
                + (salary.UtilityAllowance ?? 0)
                + (salary.MedicalAllowance ?? 0)
                + (salary.OtherAllowances ?? 0);
            decimal deductions = (salary.IncomeTax ?? 0)
                + (salary.ProfessionalTax ?? 0)
                + (salary.OtherDeductions ?? 0);
            decimal net = gross - deductions;

            _context.EmployeePayrolls.Add(new EmployeePayroll
            {
                EmployeeId = employeeId,
                SalaryId = salary.SalaryId,
                MonthYear = monthYear,
                GrossPay = gross,
                TotalDeductions = deductions,
                NetPay = net,
                Status = "Draft",
                CreatedBy = UserId.ToString(),
                CreatedDate = DateTime.Now
            });
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Payroll created!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Edit")]
        public async Task<IActionResult> Edit(int payrollId, decimal? totalDeductions)
        {
            var p = await _context.EmployeePayrolls.FindAsync(payrollId);
            if (p == null) return Json(new { success = false, message = "Not found" });
            if (p.Status != "Draft" && p.Status != "Submitted")
                return Json(new { success = false, message = "Only Draft/Submitted payroll can be edited." });
            if (totalDeductions.HasValue && totalDeductions.Value < 0)
                return Json(new { success = false, message = "Invalid deductions." });

            if (totalDeductions.HasValue)
            {
                p.TotalDeductions = totalDeductions.Value;
                p.NetPay = p.GrossPay - totalDeductions.Value;
            }
            p.UpdatedBy = UserId.ToString();
            p.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Payroll updated!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var p = await _context.EmployeePayrolls.FindAsync(id);
            if (p == null) return Json(new { success = false, message = "Not found" });
            if (p.Status != "Draft")
                return Json(new { success = false, message = "Only Draft payroll can be deleted." });

            p.Status = "Cancelled";
            p.UpdatedBy = UserId.ToString();
            p.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Payroll cancelled." });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Approve")]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var p = await _context.EmployeePayrolls.FindAsync(id);
            if (p == null) return Json(new { success = false, message = "Not found" });

            string old = p.Status ?? "Draft";
            switch (old)
            {
                case "Draft":
                    p.Status = "Submitted";
                    break;
                case "Submitted":
                    if (p.CreatedBy == UserId.ToString())
                        return Json(new { success = false, message = "You cannot approve your own payroll." });
                    p.Status = "Approved";
                    p.ApprovedBy = UserId.ToString();
                    p.ApprovedDate = DateTime.Now;
                    break;
                case "Approved":
                    p.Status = "Paid";
                    p.PaymentDate = DateTime.Now;
                    break;
                default:
                    return Json(new { success = false, message = "Invalid status transition." });
            }

            p.UpdatedBy = UserId.ToString();
            p.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Status: " + old + " -> " + p.Status });
        }

        [HttpGet]
        public async Task<IActionResult> Profile(int id)
        {
            var p = await _context.EmployeePayrolls.Include(x => x.Employee)
                .FirstOrDefaultAsync(x => x.PayrollId == id);
            if (p == null) return NotFound();

            if (int.TryParse(p.CreatedBy, out int cid))
            {
                var creator = await _context.Users.FindAsync(cid);

            }


            if (int.TryParse(p.ApprovedBy, out int aid))
            {
                var approver = await _context.Users.FindAsync(aid);
                ViewBag.ApprovedByName = approver?.UserName ?? "N/A";
            }
            else ViewBag.ApprovedByName = "-";

            if (int.TryParse(p.UpdatedBy, out int uid))
            {
                var upd = await _context.Users.FindAsync(uid);

            }


            return View("Profile", p);
        }
    }
}


