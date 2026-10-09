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

namespace ERPWMS.Web.Controllers.Workflow
{
    [Authorize]
    public class WorkflowStageController : Controller
    {
        private readonly AppDbContext _context;

        public WorkflowStageController(AppDbContext c)
        {
            _context = c;
        }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        // ============================================================
        // INDEX
        // ============================================================
        [HttpGet]
        public IActionResult Index() => View();

        // ============================================================
        // STATISTICS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetStatistics(int? workflowDefId)
        {
            var q = _context.WorkflowStages.Where(s => s.IsDeleted != true);
            if (workflowDefId.HasValue && workflowDefId > 0)
                q = q.Where(s => s.WorkflowDefId == workflowDefId);

            var l = await q.ToListAsync();

            return Json(new
            {
                total = l.Count,
                active = l.Count(s => s.IsActive == true),
                inactive = l.Count(s => s.IsActive != true),
                addedThisMonth = l.Count(s =>
                    s.CreatedDate.HasValue &&
                    s.CreatedDate.Value.Month == DateTime.Now.Month &&
                    s.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        // ============================================================
        // GET ALL
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetAll(int? workflowDefId)
        {
            var q = _context.WorkflowStages
                .Include(s => s.WorkflowDef)
                .Where(s => s.IsDeleted != true);

            if (workflowDefId.HasValue && workflowDefId > 0)
                q = q.Where(s => s.WorkflowDefId == workflowDefId);

            var l = await q
                .OrderBy(s => s.StageOrder)
                .Select(s => new
                {
                    stageId = s.StageId,
                    stageName = s.StageName,
                    stageCode = s.StageCode,
                    workflowName = s.WorkflowDef != null ? s.WorkflowDef.WorkflowName : "-",
                    stageOrder = s.StageOrder,
                    isStart = s.IsStart ?? false,
                    isEnd = s.IsEnd ?? false,
                    slaHours = s.SladeadlineHours,          // ✅ FIX: SladeadlineHours
                    isActive = s.IsActive ?? false,
                    createdDate = s.CreatedDate
                })
                .ToListAsync();
            return Json(l);
        }

        // ============================================================
        // GET WORKFLOWS
        // ============================================================
        [HttpGet]
        public async Task<IActionResult> GetWorkflows()
        {
            var l = await _context.WorkflowDefinitions
                .Where(d => d.IsActive == true && d.IsDeleted != true)
                .Select(d => new { workflowDefId = d.WorkflowDefId, workflowName = d.WorkflowName })
                .ToListAsync();
            return Json(l);
        }

        // ============================================================
        // ADD / EDIT PARTIALS
        // ============================================================
        [HttpGet]
        public IActionResult AddPartial() => PartialView("_AddForm");

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var s = await _context.WorkflowStages.FindAsync(id);
            if (s == null) return NotFound();
            return PartialView("_EditForm", s);
        }

        // ============================================================
        // REGISTER
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            int workflowDefId,
            string stageName,
            string? stageCode,
            int stageOrder,
            bool isStart,
            bool isEnd,
            int slaDeadlineHours)
        {
            try
            {
                if (workflowDefId <= 0)
                    return Json(new { success = false, message = "Workflow required." });
                if (string.IsNullOrWhiteSpace(stageName))
                    return Json(new { success = false, message = "Stage name required." });

                _context.WorkflowStages.Add(new WorkflowStage
                {
                    WorkflowDefId = workflowDefId,
                    StageName = stageName.Trim(),
                    StageCode = stageCode ?? "",
                    StageOrder = stageOrder,
                    IsStart = isStart,
                    IsEnd = isEnd,
                    SladeadlineHours = slaDeadlineHours > 0 ? slaDeadlineHours : (int?)null,  // ✅ FIX
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Stage created!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // ============================================================
        // EDIT
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int stageId,
            string stageName,
            string? stageCode,
            int stageOrder,
            bool isStart,
            bool isEnd,
            int slaDeadlineHours,
            bool isActive = true)
        {
            try
            {
                var s = await _context.WorkflowStages.FindAsync(stageId);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                s.StageName = stageName.Trim();
                s.StageCode = stageCode ?? "";
                s.StageOrder = stageOrder;
                s.IsStart = isStart;
                s.IsEnd = isEnd;
                s.SladeadlineHours = slaDeadlineHours > 0 ? slaDeadlineHours : (int?)null;  // ✅ FIX
                s.IsActive = isActive;
                s.UpdatedBy = Uid().ToString();
                s.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Stage updated!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = (ex.InnerException ?? ex).Message });
            }
        }

        // ============================================================
        // DELETE
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var s = await _context.WorkflowStages.FindAsync(id);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                s.IsDeleted = true;
                s.IsActive = false;
                s.DeletedBy = Uid().ToString();
                s.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // TOGGLE
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle(int id)
        {
            try
            {
                var s = await _context.WorkflowStages.FindAsync(id);
                if (s == null)
                    return Json(new { success = false, message = "Not found" });

                s.IsActive = !(s.IsActive ?? false);
                s.UpdatedBy = Uid().ToString();
                s.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = s.IsActive == true ? "Activated." : "Deactivated." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ============================================================
        // ALIASES
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> QuickAdd(int workflowDefId, string stageName, string stageCode, int stageOrder, int slaDeadlineHours, bool isStart, bool isEnd)
            => Register(workflowDefId, stageName, stageCode, stageOrder, isStart, isEnd, slaDeadlineHours);

        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> QuickEdit(int stageId, string stageName, string stageCode, int stageOrder, int slaDeadlineHours, bool isStart, bool isEnd, bool isActive = true)
            => Edit(stageId, stageName, stageCode, stageOrder, isStart, isEnd, slaDeadlineHours, isActive);

        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> QuickDelete(int id) => Delete(id);

        [HttpPost, ValidateAntiForgeryToken]
        public Task<IActionResult> QuickToggle(int id) => Toggle(id);
    }
}