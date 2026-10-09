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
    [Authorize]
    public class PayslipController : Controller
    {
        private readonly AppDbContext _context;
        public PayslipController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.Payslips
                .Include(p => p.Employee)
                .Where(p => p.IsDeleted != true)
                .OrderByDescending(p => p.PayslipId)
                .Take(100)
                .Select(p => new {
                    payslipId = p.PayslipId, payslipNo = p.PayslipNo,
                    employeeName = p.Employee.FirstName + " " + p.Employee.LastName,
                    employeeCode = p.Employee.EmployeeCode,
                    year = p.Year, month = p.Month,
                    grossPay = p.GrossPay, totalDeductions = p.TotalDeductions, netPay = p.NetPay,
                    generatedDate = p.GeneratedDate.HasValue ? p.GeneratedDate.Value.ToString("dd-MMM-yyyy") : null
                }).ToListAsync();
            return Json(l);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateAll(int runId)
        {
            try
            {
                var run = await _context.PayrollRuns.FirstOrDefaultAsync(r => r.RunId == runId);
                if (run == null) return Json(new { success = false, message = "Payroll run not found." });
                if (run.Status != "Approved" && run.Status != "Locked" && run.Status != "Posted")
                    return Json(new { success = false, message = "Payroll must be Approved/Posted first." });

                var registers = await _context.PayrollRegisters
                    .Where(r => r.RunId == runId)
                    .ToListAsync();

                int created = 0;
                foreach (var reg in registers)
                {
                    var exists = await _context.Payslips.AnyAsync(p =>
                        p.RegisterId == reg.RegisterId && p.IsDeleted != true);
                    if (exists) continue;

                    _context.Payslips.Add(new Payslip {
                        PayslipNo = $"PS-{run.Year}{run.Month:D2}-{reg.EmployeeId:D5}",
                        RegisterId = reg.RegisterId,
                        EmployeeId = reg.EmployeeId,
                        Year = run.Year, Month = run.Month,
                        GrossPay = reg.TotalEarnings,
                        TotalDeductions = reg.TotalDeductions,
                        NetPay = reg.NetPay,
                        GeneratedDate = DateTime.Now,
                        IsActive = true, IsDeleted = false,
                        CreatedBy = Uid().ToString(), CreatedDate = DateTime.Now
                    });
                    created++;
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"{created} payslips generated!" });
            } catch (Exception ex) {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }
    }
}
