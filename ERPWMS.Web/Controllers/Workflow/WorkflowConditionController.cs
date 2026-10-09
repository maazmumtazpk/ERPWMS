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
    public class WorkflowConditionController : Controller
    {
        private readonly AppDbContext _context;
        public WorkflowConditionController(AppDbContext c) { _context = c; }

        private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(cl, out var i) && i > 0) return i;
            var u = _context.Users.FirstOrDefault();
            return u != null ? u.Id : 1;
        }

        [HttpGet] public IActionResult Index() => View();

        [HttpGet]
        public async Task<IActionResult> GetStatistics(int? workflowDefId)
        {
            var q = _context.WorkflowConditions.AsQueryable();
            if (workflowDefId.HasValue && workflowDefId > 0)
                q = q.Where(c => c.WorkflowDefId == workflowDefId);
            var l = await q.ToListAsync();
            return Json(new {
                total = l.Count,
                active = l.Count(c => c.ActionOnFail == null || c.ActionOnFail == "Allow"),
                inactive = l.Count(c => c.ActionOnFail != null && c.ActionOnFail != "Allow"),
                addedThisMonth = l.Count(c => c.CreatedDate.HasValue && c.CreatedDate.Value.Month == DateTime.Now.Month && c.CreatedDate.Value.Year == DateTime.Now.Year)
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int? workflowDefId)
        {
            var q = _context.WorkflowConditions
                .Include(c => c.WorkflowDef).Include(c => c.StageFrom).Include(c => c.StageTo)
                .AsQueryable();
            if (workflowDefId.HasValue && workflowDefId > 0)
                q = q.Where(c => c.WorkflowDefId == workflowDefId);
            var l = await q.OrderBy(c => c.ConditionId).Select(c => new {
                conditionId = c.ConditionId,
                workflowName = c.WorkflowDef != null ? c.WorkflowDef.WorkflowName : "-",
                fromStage = c.StageFrom != null ? c.StageFrom.StageName : "-",
                toStage = c.StageTo != null ? c.StageTo.StageName : "-",
                conditionType = c.ConditionType,
                operatorName = c.Operator,
                value = c,
                actionOnFail = c.ActionOnFail ?? "Allow",
                createdDate = c.CreatedDate
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
        public IActionResult AddPartial() => PartialView("_AddForm");

        [HttpGet]
        public async Task<IActionResult> EditPartial(int id)
        {
            var c = await _context.WorkflowConditions.FindAsync(id);
            if (c == null) return NotFound();
            return PartialView("_EditForm", c);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int workflowDefId, int stageFromId, int stageToId, string conditionType, string operatorName, string value, string actionOnFail)
        {
            try
            {
                if (workflowDefId <= 0) return Json(new { success = false, message = "Workflow required." });
                _context.WorkflowConditions.Add(new WorkflowCondition {
                    WorkflowDefId = workflowDefId,
                    StageFromId = stageFromId,
                    StageToId = stageToId,
                    ConditionType = conditionType ?? "Field",
                    Operator = operatorName ?? "=",
                    Value = value ?? "",
                    ActionOnFail = actionOnFail,
                    CreatedBy = Uid().ToString(),
                    CreatedDate = DateTime.Now
                });
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Condition created!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int conditionId, int stageFromId, int stageToId, string conditionType, string operatorName, string value, string actionOnFail)
        {
            try
            {
                var c = await _context.WorkflowConditions.FindAsync(conditionId);
                if (c == null) return Json(new { success = false, message = "Not found" });
                c.StageFromId = stageFromId;
                c.StageToId = stageToId;
                c.ConditionType = conditionType;
                c.Operator = operatorName;
                c.Value = value;
                c.ActionOnFail = actionOnFail;
                c.UpdatedBy = Uid().ToString();
                c.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Condition updated!" });
            }
            catch (Exception ex) { return Json(new { success = false, message = (ex.InnerException ?? ex).Message }); }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var c = await _context.WorkflowConditions.FindAsync(id);
                if (c == null) return Json(new { success = false, message = "Not found" });
                _context.WorkflowConditions.Remove(c);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Deleted." });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        // --- Aliases for views ---
        [HttpPost, ValidateAntiForgeryToken] public Task<IActionResult> QuickAdd(int workflowDefId, int stageFromId, int stageToId, string conditionType, string operatorName, string value, string actionOnFail)
            => Register(workflowDefId, stageFromId, stageToId, conditionType, operatorName, value, actionOnFail);

        [HttpPost, ValidateAntiForgeryToken] public Task<IActionResult> QuickEdit(int conditionId, int stageFromId, int stageToId, string conditionType, string operatorName, string value, string actionOnFail)
            => Edit(conditionId, stageFromId, stageToId, conditionType, operatorName, value, actionOnFail);

        [HttpPost, ValidateAntiForgeryToken] public Task<IActionResult> QuickDelete(int id) => Delete(id);
    }
}
