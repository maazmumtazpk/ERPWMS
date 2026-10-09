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
    public class BonusController : Controller
    {
        private readonly AppDbContext _context;
        public BonusController(AppDbContext c) { _context = c; }

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
            var l = await _context.Bonuses
                .Include(x => x.Employee)
                .Where(x => x.IsDeleted != true)
                .OrderByDescending(x => x.BonusId)
                .Select(x => new {
                    bonusId = x.BonusId,
                    employeeName = x.Employee.FirstName + " " + x.Employee.LastName,
                    employeeCode = x.Employee.EmployeeCode,
                    bonusType = x.BonusType,
                    amount = x.Amount,
                    bonusDate = x.BonusDate.ToString("yyyy-MM-dd"),
                    status = x.Status
                }).ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var l = await _context.Employees.Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new { employeeId = e.EmployeeId, employeeCode = e.EmployeeCode, fullName = e.FirstName + " " + e.LastName })
                .OrderBy(e => e.fullName).ToListAsync();
            return Json(l);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(int employeeId, string bonusType, decimal amount, string bonusDate, string? remarks)
        {
            try
            {
                if (employeeId <= 0 || amount <= 0) return Json(new { success = false, message = "Employee and Amount required." });
                if (!DateOnly.TryParse(bonusDate, out var dt)) return Json(new { success = false, message = "Invalid date." });

                _context.Bonuses.Add(new Bonus {
                    EmployeeId = employeeId, BonusType = bonusType ?? "Performance",
                    Amount = amount, BonusDate = dt, Remarks = remarks,
                    Status = "Pending", IsActive = true, IsDeleted = false,
                    CreatedBy = Uid().ToString(), CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Bonus added!" });
            } catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var b = await _context.Bonuses.FindAsync(id);
            if (b == null) return Json(new { success = false, message = "Not found" });
            b.Status = "Approved"; b.ApprovedBy = Uid().ToString(); b.ApprovedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Approved!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var b = await _context.Bonuses.FindAsync(id);
            if (b == null) return Json(new { success = false, message = "Not found" });
            b.IsDeleted = true; b.IsActive = false;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }
    }
}
