using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Controllers.Workflow
{
    [Authorize]
    public class WorkflowDashboardController : Controller
    {
        private readonly AppDbContext _context;
        public WorkflowDashboardController(AppDbContext c) { _context = c; }

        [HttpGet] public IActionResult Index() => View();

        // KPI Stats
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var defs = await _context.WorkflowDefinitions.CountAsync(d => d.IsDeleted != true);
                var insts = await _context.WorkflowInstances.CountAsync();
                var active = await _context.WorkflowInstances.CountAsync(i => i.Status == "InProgress");
                var completed = await _context.WorkflowInstances.CountAsync(i => i.Status == "Completed");
                var rejected = await _context.WorkflowInstances.CountAsync(i => i.Status == "Rejected");
                var stages = await _context.WorkflowStages.CountAsync(s => s.IsDeleted != true);
                var assignments = await _context.WorkflowAssignments.CountAsync();
                var pendingDeadlines = await _context.WorkflowDeadlines.CountAsync(d => d.IsMet != true);

                return Json(new
                {
                    definitions = defs,
                    instances = insts,
                    active = active,
                    completed = completed,
                    rejected = rejected,
                    stages = stages,
                    assignments = assignments,
                    pendingDeadlines = pendingDeadlines
                });
            }
            catch
            {
                return Json(new { definitions = 0, instances = 0, active = 0, completed = 0, rejected = 0, stages = 0, assignments = 0, pendingDeadlines = 0 });
            }
        }

        // Recent instances list
        [HttpGet]
        public async Task<IActionResult> GetRecentInstances(int take = 10)
        {
            try
            {
                var list = await _context.WorkflowInstances
                    .Include(i => i.WorkflowDef)
                    .Include(i => i.CurrentStage)
                    .OrderByDescending(i => i.WorkflowInstId)
                    .Take(take)
                    .Select(i => new
                    {
                        workflowInstId = i.WorkflowInstId,
                        workflowName = i.WorkflowDef != null ? i.WorkflowDef.WorkflowName : "-",
                        recordType = i.RecordType ?? "",
                        recordId = i.RecordId,
                        currentStage = i.CurrentStage != null ? i.CurrentStage.StageName : "-",
                        status = i.Status ?? "InProgress",
                        startedDate = i.StartedDate,
                        createdDate = i.CreatedDate
                    })
                    .ToListAsync();
                return Json(list);
            }
            catch { return Json(new System.Collections.Generic.List<object>()); }
        }

        // Workflow summary (per definition)
        [HttpGet]
        public async Task<IActionResult> GetWorkflowSummary()
        {
            try
            {
                var list = await _context.WorkflowDefinitions
                    .Where(d => d.IsDeleted != true)
                    .Select(d => new
                    {
                        workflowDefId = d.WorkflowDefId,
                        workflowName = d.WorkflowName,
                        moduleType = d.ModuleType,
                        isActive = d.IsActive == true,
                        stages = _context.WorkflowStages.Count(s => s.WorkflowDefId == d.WorkflowDefId && s.IsDeleted != true),
                        instances = _context.WorkflowInstances.Count(i => i.WorkflowDefId == d.WorkflowDefId),
                        inProgress = _context.WorkflowInstances.Count(i => i.WorkflowDefId == d.WorkflowDefId && i.Status == "InProgress")
                    })
                    .OrderByDescending(x => x.instances)
                    .Take(10)
                    .ToListAsync();
                return Json(list);
            }
            catch { return Json(new System.Collections.Generic.List<object>()); }
        }

        // Recent logs
        [HttpGet]
        public async Task<IActionResult> GetRecentActivity(int take = 8)
        {
            try
            {
                var list = await _context.WorkflowInstanceLogs
                    .Include(l => l.Action)
                    .Include(l => l.User)
                    .Include(l => l.StageTo)
                    .OrderByDescending(l => l.LogId)
                    .Take(take)
                    .Select(l => new
                    {
                        logId = l.LogId,
                        workflowInstId = l.WorkflowInstId,
                        actionName = l.Action != null ? l.Action.ActionName : "-",
                        userName = l.User != null ? l.User.UserName : "-",
                        toStage = l.StageTo != null ? l.StageTo.StageName : "-",
                        actionDate = l.ActionDate
                    })
                    .ToListAsync();
                return Json(list);
            }
            catch { return Json(new System.Collections.Generic.List<object>()); }
        }
    }
}
