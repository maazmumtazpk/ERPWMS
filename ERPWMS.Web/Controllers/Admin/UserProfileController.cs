using ERPWMS.Database;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ERPWMS.Controllers.Admin
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly AppDbContext _context;
        public UserProfileController(AppDbContext c) { _context = c; }

        private int CurrentUserId
        {
            get
            {
                var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(cl, out int id) || id <= 0)
                    throw new UnauthorizedAccessException("User identity missing.");
                return id;
            }
        }

        private bool IsAdmin
        {
            get
            {
                return User.IsInRole("Admin")
                    || User.IsInRole("SuperAdmin")
                    || User.HasClaim("Permission", "SystemAdmin.View")
                    || User.HasClaim("Permission", "SystemAdmin.Manage");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var currentUserId = CurrentUserId;

            // Agar id provided nahi hai ya invalid hai, apna profile dikhao
            if (id <= 0) id = currentUserId;

            // Agar user apna profile nahi dekh raha, aur admin bhi nahi hai, to block
            if (id != currentUserId && !IsAdmin)
                return Forbid();

            ViewBag.UserId = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var currentUserId = CurrentUserId;

            // IDOR protection: sirf apna ya (admin ke liye) kisi ka bhi
            if (userId != currentUserId && !IsAdmin)
                return Forbid();

            var u = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (u == null) return Json(null);

            var roleName = await (from r in _context.AspNetRoles
                                  where r.Users.Any(x => x.Id == userId)
                                  select r.Name).FirstOrDefaultAsync();

            var fullName = ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim();

            return Json(new
            {
                userId = u.Id,
                userName = u.UserName,
                fullName = fullName,
                email = u.Email,
                phoneNumber = u.PhoneNumber,
                roleName = roleName,
                isActive = u.IsActive == true,
                createdDate = (DateTime?)null,
                assignments = await _context.WorkflowAssignments.CountAsync(a => a.UserId == u.Id),
                actions = await _context.WorkflowInstanceLogs.CountAsync(l => l.UserId == u.Id)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetActivity(int userId, int take = 20)
        {
            var currentUserId = CurrentUserId;

            // IDOR protection
            if (userId != currentUserId && !IsAdmin)
                return Forbid();

            if (take < 1) take = 20;
            if (take > 100) take = 100;

            var list = await _context.WorkflowInstanceLogs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.ActionDate)
                .Take(take)
                .Select(l => new
                {
                    workflowInstId = l.WorkflowInstId,
                    actionName = l.Action != null ? l.Action.ActionName : "-",
                    toStage = l.StageTo != null ? l.StageTo.StageName : "-",
                    actionDate = l.ActionDate
                }).ToListAsync();
            return Json(list);
        }
    }
}
