using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Controllers.Admin
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly AppDbContext _context;
        public UserProfileController(AppDbContext c) { _context = c; }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            if (id <= 0)
            {
                var u = await _context.Users.FirstOrDefaultAsync();
                id = u != null ? u.Id : 0;
            }
            ViewBag.UserId = id;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile(int userId)
        {
            var u = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (u == null) return Json(null);

            // Role name via many-to-many (User <-> AspNetRole)
            var roleName = await (from r in _context.AspNetRoles
                                  where r.Users.Any(x => x.Id == userId)
                                  select r.Name).FirstOrDefaultAsync();

            var fullName = ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim();

            return Json(new {
                userId = u.Id,
                userName = u.UserName,
                fullName = fullName,
                email = u.Email,
                phoneNumber = u.PhoneNumber,
                roleName = roleName,
                isActive = u.IsActive == true,
                createdDate = (DateTime?)null,
                assignments = _context.WorkflowAssignments.Count(a => a.UserId == u.Id),
                actions = _context.WorkflowInstanceLogs.Count(l => l.UserId == u.Id)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetActivity(int userId, int take = 20)
        {
            var list = await _context.WorkflowInstanceLogs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.ActionDate)
                .Take(take)
                .Select(l => new {
                    workflowInstId = l.WorkflowInstId,
                    actionName = l.Action != null ? l.Action.ActionName : "-",
                    toStage = l.StageTo != null ? l.StageTo.StageName : "-",
                    actionDate = l.ActionDate
                }).ToListAsync();
            return Json(list);
        }
    }
}
