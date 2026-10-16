using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Controllers.Workflow
{
    [Authorize]
    public class WorkflowTransitionController : Controller
    {
        private readonly AppDbContext _context;
        public WorkflowTransitionController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }

        [HttpGet] public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetStatistics(int? workflowDefId)
        {
            var q = _context.WorkflowTransitions.AsQueryable();
            if (workflowDefId.HasValue && workflowDefId > 0)
                q = q.Where(t => t.WorkflowDefId == workflowDefId);
            var l = await q.ToListAsync();
            return Json(new {
                total = l.Count,
                active = l.Count(t => t.IsAllowed == true),
                inactive = l.Count(t => t.IsAllowed != true),
                addedThisMonth = l.Count(t => t.CreatedDate.HasValue && t.CreatedDate.Value.Month == DateTime.Now.Month && t.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int? workflowDefId)
        {
            var q = _context.WorkflowTransitions
                .Include(t => t.StageFrom).Include(t => t.StageTo)
                .Include(t => t.Action).Include(t => t.WorkflowDef)
                .AsQueryable();
            if (workflowDefId.HasValue && workflowDefId > 0)
                q = q.Where(t => t.WorkflowDefId == workflowDefId);
            var l = await q.OrderBy(t => t.TransitionId).Select(t => new {
                transitionId = t.TransitionId,
                workflowName = t.WorkflowDef != null ? t.WorkflowDef.WorkflowName : "-",
                fromStage = t.StageFrom != null ? t.StageFrom.StageName : "-",
                toStage = t.StageTo != null ? t.StageTo.StageName : "-",
                actionName = t.Action != null ? t.Action.ActionName : "-",
                isAllowed = t.IsAllowed ?? false,
                createdDate = t.CreatedDate
            }).ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkflows()
        {
            var l = await _context.WorkflowDefinitions
                .Where(d => d.IsActive == true && d.IsDeleted != true)
                .Select(d => new { workflowDefId = d.WorkflowDefId, workflowName = d.WorkflowName })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetStages(int workflowDefId)
        {
            var l = await _context.WorkflowStages
                .Where(s => s.WorkflowDefId == workflowDefId && s.IsDeleted != true)
                .Select(s => new { stageId = s.StageId, stageName = s.StageName })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public async Task<IActionResult> GetActions()
        {
            var l = await _context.WorkflowActions
                .Where(a => a.IsActive == true)
                .Select(a => new { actionId = a.ActionId, actionName = a.ActionName })
                .ToListAsync();
            return Json(l);
        }

        [HttpGet]
        public IActionResult AddPartial() => PartialView("_AddForm");

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var t = await _context.WorkflowTransitions.FindAsync(id);
            if (t == null) return NotFound();
            return PartialView("_EditForm", t);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int workflowDefId, int stageFromId, int stageToId, int actionId, bool isAllowed)
        {
            try
            {
                if (workflowDefId <= 0 || stageFromId <= 0 || stageToId <= 0 || actionId <= 0)
                    return Json(new { success = false, message = "All fields required." });
                _context.WorkflowTransitions.Add(new WorkflowTransition {
                    WorkflowDefId = workflowDefId,
                    StageFromId = stageFromId,
                    StageToId = stageToId,
                    ActionId = actionId,
                    IsAllowed = isAllowed,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Transition created!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int transitionId, int stageFromId, int stageToId, int actionId, bool isAllowed)
        {
            try
            {
                var t = await _context.WorkflowTransitions.FindAsync(transitionId);
                if (t == null) return Json(new { success = false, message = "Not found" });
                t.StageFromId = stageFromId;
                t.StageToId = stageToId;
                t.ActionId = actionId;
                t.IsAllowed = isAllowed;
                t.UpdatedBy = Uid().ToString();
                t.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Transition updated!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var t = await _context.WorkflowTransitions.FindAsync(id);
                if (t == null) return Json(new { success = false, message = "Not found" });
                return Json(new { success = false, message = "Cannot delete transitions. Disable it instead." });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> Toggle(int id)
        {
            try
            {
                var t = await _context.WorkflowTransitions.FindAsync(id);
                if (t == null) return Json(new { success = false, message = "Not found" });
                t.IsAllowed = !(t.IsAllowed ?? false);
                t.UpdatedBy = Uid().ToString();
                t.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = t.IsAllowed == true ? "Allowed." : "Disallowed." });
            }
            catch (Exception ex) { return Json(new { success = false, message = "Operation failed." }); }
        }

        // --- Aliases for views ---
        [HttpPost, ValidateAntiForgeryToken] public Task<IActionResult> QuickAdd(int workflowDefId, int stageFromId, int stageToId, int actionId, bool isAllowed)
            => Register(workflowDefId, stageFromId, stageToId, actionId, isAllowed);

        [HttpPost, ValidateAntiForgeryToken] public Task<IActionResult> QuickEdit(int transitionId, int stageFromId, int stageToId, int actionId, bool isAllowed)
            => Edit(transitionId, stageFromId, stageToId, actionId, isAllowed);

        [HttpPost, ValidateAntiForgeryToken] public Task<IActionResult> QuickDelete(int id) => Delete(id);
        [HttpPost, ValidateAntiForgeryToken] public Task<IActionResult> QuickToggle(int id) => Toggle(id);
    }
}
  
