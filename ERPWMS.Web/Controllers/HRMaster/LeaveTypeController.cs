using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Web.Controllers.HRMaster
{
    [Authorize]
    public class LeaveTypeController : Controller
    {
        private readonly AppDbContext _context;
        public LeaveTypeController(AppDbContext c) { _context = c; }

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
            var l = await _context.LeaveTypes
                .Where(x => x.IsDeleted != true)
                .OrderBy(x => x.LeaveName)
                .Select(x => new {
                    leaveTypeId = x.LeaveTypeId,
                    leaveCode = x.LeaveCode,
                    leaveName = x.LeaveName,
                    category = x.Category,
                    annualEntitlement = x.AnnualEntitlement,
                    isPaid = x.IsPaid,
                    allowCarryForward = x.AllowCarryForward,
                    maxConsecutiveDays = x.MaxConsecutiveDays,
                    approvalRequired = x.ApprovalRequired,
                    isActive = x.IsActive
                }).ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            var x = await _context.LeaveTypes.FindAsync(id);
            if (x == null) return NotFound();
            return Json(new {
                leaveTypeId = x.LeaveTypeId,
                leaveCode = x.LeaveCode,
                leaveName = x.LeaveName,
                category = x.Category,
                annualEntitlement = x.AnnualEntitlement,
                isPaid = x.IsPaid,
                allowCarryForward = x.AllowCarryForward,
                maxCarryForwardDays = x.MaxCarryForwardDays,
                maxConsecutiveDays = x.MaxConsecutiveDays,
                approvalRequired = x.ApprovalRequired,
                documentRequired = x.DocumentRequired,
                noticeDays = x.NoticeDays,
                description = x.Description,
                isActive = x.IsActive
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Save(int leaveTypeId, string leaveCode, string leaveName, string category,
            int annualEntitlement, bool isPaid, bool allowCarryForward, int maxCarryForwardDays,
            int maxConsecutiveDays, bool approvalRequired, bool documentRequired, int noticeDays,
            string? description, bool isActive)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(leaveName)) return Json(new { success = false, message = "Name required." });
                if (string.IsNullOrWhiteSpace(leaveCode)) return Json(new { success = false, message = "Code required." });

                var exists = await _context.LeaveTypes.AnyAsync(x =>
                    x.LeaveCode == leaveCode && x.LeaveTypeId != leaveTypeId && x.IsDeleted != true);
                if (exists) return Json(new { success = false, message = "Code already exists." });

                LeaveType entity;
                if (leaveTypeId > 0)
                {
                    entity = await _context.LeaveTypes.FindAsync(leaveTypeId);
                    if (entity == null) return Json(new { success = false, message = "Not found." });
                    entity.LeaveCode = leaveCode; entity.LeaveName = leaveName; entity.Category = category;
                    entity.AnnualEntitlement = annualEntitlement; entity.IsPaid = isPaid;
                    entity.AllowCarryForward = allowCarryForward; entity.MaxCarryForwardDays = maxCarryForwardDays;
                    entity.MaxConsecutiveDays = maxConsecutiveDays; entity.ApprovalRequired = approvalRequired;
                    entity.DocumentRequired = documentRequired; entity.NoticeDays = noticeDays;
                    entity.Description = description; entity.IsActive = isActive;
                    entity.UpdatedBy = Uid().ToString(); entity.UpdatedDate = DateTime.Now;
                }
                else
                {
                    entity = new LeaveType {
                        LeaveCode = leaveCode, LeaveName = leaveName, Category = category,
                        AnnualEntitlement = annualEntitlement, IsPaid = isPaid,
                        AllowCarryForward = allowCarryForward, MaxCarryForwardDays = maxCarryForwardDays,
                        MaxConsecutiveDays = maxConsecutiveDays, ApprovalRequired = approvalRequired,
                        DocumentRequired = documentRequired, NoticeDays = noticeDays,
                        Description = description, IsActive = isActive, IsDeleted = false,
                        CreatedBy = Uid().ToString(), CreatedDate = DateTime.Now
                    };
                    _context.LeaveTypes.Add(entity);
                }
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = leaveTypeId > 0 ? "Updated!" : "Created!" });
            } catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var x = await _context.LeaveTypes.FindAsync(id);
            if (x == null) return Json(new { success = false, message = "Not found" });
            x.IsDeleted = true; x.IsActive = false;
            x.UpdatedBy = Uid().ToString(); x.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "Deleted." });
        }
    }
}
