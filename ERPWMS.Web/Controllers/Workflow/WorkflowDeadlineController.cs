using ERPWMS.Database.Models;using Microsoft.AspNetCore.Authorization;using Microsoft.AspNetCore.Mvc;using Microsoft.EntityFrameworkCore;using System;using System.Linq;using System.Security.Claims;using System.Threading.Tasks;
using ERPWMS.Database;
namespace ERPWMS.Controllers.Workflow{[Authorize]public class WorkflowDeadlineController:Controller{private readonly AppDbContext _context;public WorkflowDeadlineController(AppDbContext c){_context=c;}
private int Uid()
        {
            var cl = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(cl, out var i) || i <= 0)
                throw new UnauthorizedAccessException("User identity missing.");
            return i;
        }
[HttpGet]public IActionResult Index()=>View();
[HttpGet]public async Task<IActionResult> GetStatistics(){var l=await _context.WorkflowDeadlines.ToListAsync();return Json(new{total=l.Count,active=l.Count(d=>d.IsMet==true),inactive=l.Count(d=>d.IsMet!=true),addedThisMonth=l.Count(d=>d.CreatedDate.HasValue&&d.CreatedDate.Value.Month==DateTime.Now.Month&&d.CreatedDate.Value.Year==DateTime.Now.Year)});}
[HttpGet]public async Task<IActionResult> GetAll(){var l=await _context.WorkflowDeadlines.Include(d=>d.WorkflowInst).Include(d=>d.Stage).OrderBy(d => d.DeadlineId).Select(d=>new{deadlineId=d.DeadlineId,workflowInstId=d.WorkflowInstId,stageName=d.Stage!=null?d.Stage.StageName:"-",dueDate=d.DueDate,escalatedAt=d.EscalatedAt,isMet=d.IsMet??false,createdDate=d.CreatedDate}).ToListAsync();return Json(l);}
[HttpGet]public async Task<IActionResult> GetInstances(){var l=await _context.WorkflowInstances.Select(i=>new{workflowInstId=i.WorkflowInstId,display="Instance #"+i.WorkflowInstId}).Take(500).ToListAsync();return Json(l);}
[HttpGet]public async Task<IActionResult> GetStages(){var l=await _context.WorkflowStages.Where(s=>s.IsDeleted!=true).Select(s=>new{stageId=s.StageId,stageName=s.StageName}).ToListAsync();return Json(l);}
[HttpGet]public IActionResult AddPartial()=>PartialView("_AddForm");
[HttpGet]public async Task<IActionResult> EditPartial(int id){var d=await _context.WorkflowDeadlines.FindAsync(id);if(d==null)return NotFound();return PartialView("_EditForm",d);}
[HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Register(int workflowInstId,int stageId,DateTime dueDate){try{if(workflowInstId<=0||stageId<=0)return Json(new{success=false,message="Instance and stage required."});_context.WorkflowDeadlines.Add(new WorkflowDeadline{WorkflowInstId=workflowInstId,StageId=stageId,DueDate=dueDate,IsMet=false,CreatedBy=Uid().ToString(),CreatedDate=DateTime.Now});await _context.SaveChangesAsync();return Json(new{success=true,message="Deadline created!"});}catch(Exception ex){return Json(new{success=false,message=(ex.InnerException??ex).Message});}}
[HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Edit(int deadlineId,DateTime dueDate,bool isMet){try{var d=await _context.WorkflowDeadlines.FindAsync(deadlineId);if(d==null)return Json(new{success=false});d.DueDate=dueDate;d.IsMet=isMet;d.UpdatedBy=Uid().ToString();d.UpdatedDate=DateTime.Now;await _context.SaveChangesAsync();return Json(new{success=true,message="Deadline updated!"});}catch(Exception ex){return Json(new{success=false,message=(ex.InnerException??ex).Message});}}
[HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Delete(int id){try{var d=await _context.WorkflowDeadlines.FindAsync(id);if(d==null)return Json(new{success=false});return Json(new { success = false, message = "Cannot delete deadlines." });await _context.SaveChangesAsync();return Json(new{success=true,message="Deleted."});}catch(Exception ex){return Json(new{success=false,message = "Operation failed."});}}
[HttpPost,ValidateAntiForgeryToken]public async Task<IActionResult> Toggle(int id){try{var d=await _context.WorkflowDeadlines.FindAsync(id);if(d==null)return Json(new{success=false});d.IsMet=!(d.IsMet??false);d.UpdatedBy=Uid().ToString();d.UpdatedDate=DateTime.Now;await _context.SaveChangesAsync();return Json(new{success=true,message=d.IsMet==true?"Marked met.":"Marked unmet."});}catch(Exception ex){return Json(new{success=false,message = "Operation failed."});}}
}}

