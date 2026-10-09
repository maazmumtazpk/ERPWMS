using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Interfaces;
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
    public class LatePolicyController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILatePolicyEngine _engine;

        public LatePolicyController(AppDbContext c, ILatePolicyEngine engine)
        {
            _context = c;
            _engine = engine;
        }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var list = await _context.Employees
                .Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new {
                    employeeId = e.EmployeeId,
                    employeeCode = e.EmployeeCode,
                    fullName = e.FirstName + " " + e.LastName
                })
                .OrderBy(e => e.fullName)
                .ToListAsync();
            return Json(list);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Evaluate(int employeeId, int year, int month)
        {
            try
            {
                if (employeeId <= 0)
                    return Json(new { success = false, message = "Employee required." });

                var result = await _engine.EvaluateAsync(employeeId, year, month);

                return Json(new
                {
                    success = true,
                    totalLateDays = result.TotalLateDays,
                    totalLateMinutes = result.TotalLateMinutes,
                    totalEarlyDays = result.TotalEarlyDays,
                    totalEarlyMinutes = result.TotalEarlyMinutes,
                    lateHalfDays = result.LateHalfDays,
                    lateFullDays = result.LateFullDays,
                    earlyHalfDays = result.EarlyHalfDays,
                    totalDeductionDays = result.TotalDeductionDays,
                    warningLevel = result.WarningLevel,
                    summary = result.Summary
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }
    }
}
