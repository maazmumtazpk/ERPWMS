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
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        private const decimal MaxBonusMultiplier = 12m;
        private const decimal ApprovalThreshold = 50000m;

        public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var l = await _context.Bonuses.Include(x => x.Employee)
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
        public async Task<IActionResult> GetEmployees() =>
            Json(await _context.Employees.Where(e => e.IsDeleted != true && e.IsActive == true)
                .Select(e => new {
                    employeeId = e.EmployeeId,
                    employeeCode = e.EmployeeCode,
                    fullName = e.FirstName + " " + e.LastName
                }).OrderBy(e => e.fullName).ToListAsync());

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Edit")]
        public async Task<IActionResult> Save(int employeeId, string bonusType, decimal amount,
            string bonusDate, string? remarks)
        {
            try
            {
                if (employeeId <= 0) return Json(new { success = false, message = "Employee required." });
                if (amount <= 0) return Json(new { success = false, message = "Amount must be > 0." });
                if (!DateOnly.TryParse(bonusDate, out var dt)) return Json(new { success = false, message = "Invalid date." });

                if (dt > DateOnly.FromDateTime(DateTime.Today.AddDays(7)))
                    return Json(new { success = false, message = "Cannot set bonus for future date." });

                var allowedTypes = new[] { "Performance", "Festival", "Annual", "Referral", "Spot" };
                if (string.IsNullOrWhiteSpace(bonusType) || !allowedTypes.Contains(bonusType))
                    return Json(new { success = false, message = "Invalid bonus type." });

                var emp = await _context.Employees.AsNoTracking()
                    .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.IsActive == true && e.IsDeleted != true);
                if (emp == null) return Json(new { success = false, message = "Employee not found." });

                decimal maxBonus = emp.BaseSalary * MaxBonusMultiplier;
                if (amount > maxBonus)
                    return Json(new { success = false, message = $"Bonus cannot exceed {MaxBonusMultiplier}x salary ({maxBonus:N0})." });

                var dup = await _context.Bonuses.AnyAsync(b => b.EmployeeId == employeeId
                    && b.BonusType == bonusType
                    && b.BonusDate.Month == dt.Month
                    && b.BonusDate.Year == dt.Year
                    && b.IsDeleted != true);
                if (dup)
                    return Json(new { success = false, message = "Same bonus type already exists this month." });

                _context.Bonuses.Add(new Bonus
                {
                    EmployeeId = employeeId,
                    BonusType = bonusType,
                    Amount = amount,
                    BonusDate = dt,
                    Remarks = remarks,
                    Status = "Pending",
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Bonus added!" });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return Json(new { success = false, message = "Operation failed." });
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var b = await _context.Bonuses.FindAsync(id);
            if (b == null) return Json(new { success = false, message = "Not found" });
            if (b.Status != "Pending") return Json(new { success = false, message = "Only pending can be approved." });

            var currentUid = Uid().ToString();
            if (b.CreatedBy == currentUid)
                return Json(new { success = false, message = "You cannot approve your own bonus entry." });

            if (b.Amount > ApprovalThreshold && !User.HasClaim("Permission", "Payroll.ApproveLarge"))
                return Json(new { success = false, message = $"Bonus above {ApprovalThreshold:N0} requires senior approval." });

            b.Status = "Approved";
            b.ApprovedBy = currentUid;
            b.ApprovedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Approved!" });
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "Payroll.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var b = await _context.Bonuses.FindAsync(id);
            if (b == null) return Json(new { success = false, message = "Not found" });

            if (b.Status == "Approved")
                return Json(new { success = false, message = "Approved bonuses cannot be deleted." });

            b.IsDeleted = true;
            b.IsActive = false;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }
    }
}


