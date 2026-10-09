using ERPWMS.Domain.Services;
using ERPWMS.Domain.Services.Workflow;
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
    [Authorize(Policy = "Workflow.View")]
    public class WorkflowInstanceLogController : Controller
    {
        private readonly IWorkflowEngineService _engineService;
        private readonly AppDbContext _context;

        public WorkflowInstanceLogController(
            IWorkflowEngineService engineService,
            AppDbContext context)
        {
            _engineService = engineService;
            _context = context;
        }

        // ═══════════════════════════════════════════════════════
        // INDEX
        // ═══════════════════════════════════════════════════════
        [HttpGet]
        public IActionResult Index() => View();

        // ═══════════════════════════════════════════════════════
        // STATISTICS  (no CreatedDate — uses ActionDate)
        // ═══════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var logs = await _context.WorkflowInstanceLogs.ToListAsync();
                var now = DateTime.Now;

                return Json(new {
                    total = logs.Count,
                    active = logs.Count(l =>
                        l.ActionDate.HasValue &&
                        l.ActionDate.Value.Month == now.Month &&
                        l.ActionDate.Value.Year == now.Year),
                    inactive = 0,
                    addedThisMonth = logs.Count(l =>
                        l.ActionDate.HasValue &&
                        l.ActionDate.Value.Month == now.Month &&
                        l.ActionDate.Value.Year == now.Year)
                });
            }
            catch
            {
                return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0 });
            }
        }

        // ═══════════════════════════════════════════════════════
        // GET ALL
        // ═══════════════════════════════════════════════════════
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var list = await _context.WorkflowInstanceLogs
                    .Include(x => x.Action)
                    .Include(x => x.StageFrom)
                    .Include(x => x.StageTo)
                    .Include(x => x.User)
                    .Include(x => x.WorkflowInst)
                    .OrderBy(x => x.LogId)
                    .Select(x => new {
                        logId = x.LogId,
                        workflowInstId = x.WorkflowInstId,
                        fromStage = x.StageFrom != null ? x.StageFrom.StageName : "-",
                        toStage = x.StageTo != null ? x.StageTo.StageName : "-",
                        actionName = x.Action != null ? x.Action.ActionName : "-",
                        userName = x.User != null ? x.User.UserName : "-",
                        comments = x.Comments ?? "",
                        actionDate = x.ActionDate
                    })
                    .ToListAsync();

                return Json(list);
            }
            catch
            {
                return Json(new System.Collections.Generic.List<object>());
            }
        }
    }
}
