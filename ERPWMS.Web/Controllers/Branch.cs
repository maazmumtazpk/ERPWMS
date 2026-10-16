using ERPWMS.Application.Common;
using ERPWMS.Application.DTOs.Branch;
using ERPWMS.Infrastructure.Data;
using ERPWMS.Database.Models;
using ERPWMS.Web.ViewModels.Branch;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Web.Controllers
{
    /// <summary>
    /// Controller for branch management operations
    /// </summary>
    [Authorize]
    public class BranchController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ILoggingService _logger;
        private readonly AppDbContext _context;

        public BranchController(
            IMediator mediator,
            ILoggingService logger,
            AppDbContext context)
        {
            _mediator = mediator;
            _logger = logger;
            _context = context;
        }

        // ============================================================
        // INDEX
        // ============================================================
        [HttpGet]
        [Authorize(Policy = "BranchView")]
        public IActionResult Index()
        {
            return View();
        }

        // ============================================================
        // REGISTER - GET
        // ============================================================
        [HttpGet]
        [Authorize(Policy = "BranchCreate")]
        public IActionResult Register()
        {
            return View(new RegisterBranchViewModel());
        }

        // ============================================================
        // REGISTER - POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "BranchCreate")]
        public async Task<IActionResult> Register(RegisterBranchViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var request = new RegisterBranchRequest
                {
                    BranchName = viewModel.BranchName,
                    BranchCode = viewModel.BranchCode,
                    Address = viewModel.Address,
                    Phone = viewModel.Phone,
                    Email = viewModel.Email,
                    CompanyId = viewModel.CompanyId ?? 0          // âœ… FIX
                };

                var response = await _mediator.Send(request);
                TempData["SuccessMessage"] = $"Branch '{response.BranchName}' registered successfully.";

                return RedirectToAction(nameof(Profile), new { id = response.BranchId });
            }
            catch (Domain.Exceptions.BranchAlreadyExistsException ex)
            {
                ModelState.AddModelError("BranchName", ex.Message);
                return View(viewModel);
            }
            catch (Domain.Exceptions.CompanyNotFoundExceptionForBranch ex)
            {
                ModelState.AddModelError("CompanyId", ex.Message);
                return View(viewModel);
            }
            catch (Domain.Exceptions.CompanyInactiveException ex)
            {
                ModelState.AddModelError("CompanyId", ex.Message);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error registering branch", ex);
                ModelState.AddModelError("", "An error occurred while registering the branch.");
                return View(viewModel);
            }
        }

        // ============================================================
        // PROFILE
        // ============================================================
        [HttpGet]
        [Authorize(Policy = "BranchView")]
        public async Task<IActionResult> Profile(int id)
        {
            try
            {
                var query = new GetBranchProfileQuery { BranchId = id };
                var profile = await _mediator.Send(query);

                var viewModel = new BranchViewModel
                {
                    BranchId = profile.BranchId,
                    BranchName = profile.BranchName,
                    BranchCode = profile.BranchCode ?? string.Empty,
                    Address = profile.Address,
                    Phone = profile.Phone,
                    Email = profile.Email,
                    CompanyId = profile.CompanyId,
                    CompanyName = profile.CompanyName,
                    IsActive = profile.IsActive,
                    IsDeleted = profile.IsDeleted,
                    CreatedDate = profile.CreatedDate,
                    CreatedBy = profile.CreatedBy,
                    UpdatedDate = profile.UpdatedDate,
                    UpdatedBy = profile.UpdatedBy,
                    EmployeeCount = profile.EmployeeCount,
                    WarehouseCount = profile.WarehouseCount
                };

                ViewBag.Warehouses = await _context.Warehouses
                    .Where(w => w.BranchId == id && w.IsDeleted != true)
                    .OrderBy(w => w.WarehouseName)
                    .ToListAsync();

                return View(viewModel);
            }
            catch (Domain.Exceptions.BranchNotFoundException)
            {
                return NotFound();
            }
        }

        // ============================================================
        // EDIT - GET
        // ============================================================
        [HttpGet]
        [Authorize(Policy = "BranchUpdate")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var query = new GetBranchProfileQuery { BranchId = id };
                var profile = await _mediator.Send(query);

                var viewModel = new RegisterBranchViewModel
                {
                    BranchId = profile.BranchId,
                    BranchName = profile.BranchName,
                    BranchCode = profile.BranchCode ?? string.Empty,
                    Address = profile.Address,
                    Phone = profile.Phone,
                    Email = profile.Email,
                    CompanyId = profile.CompanyId,
                    CompanyName = profile.CompanyName,
                    IsActive = profile.IsActive
                };

                return View(viewModel);
            }
            catch (Domain.Exceptions.BranchNotFoundException)
            {
                return NotFound();
            }
        }

        // ============================================================
        // EDIT - POST
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "BranchUpdate")]
        public async Task<IActionResult> Edit(RegisterBranchViewModel viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            try
            {
                var request = new ChangeBranchProfileRequest
                {
                    BranchId = viewModel.BranchId ?? 0,          // âœ… FIX
                    BranchName = viewModel.BranchName,
                    BranchCode = viewModel.BranchCode,
                    Address = viewModel.Address,
                    Phone = viewModel.Phone,
                    Email = viewModel.Email
                };

                var response = await _mediator.Send(request);
                TempData["SuccessMessage"] = $"Branch profile updated successfully.";

                return RedirectToAction(nameof(Profile), new { id = response.BranchId });
            }
            catch (Domain.Exceptions.BranchNotFoundException)
            {
                return NotFound();
            }
            catch (Domain.Exceptions.BranchAlreadyExistsException ex)
            {
                ModelState.AddModelError("BranchName", ex.Message);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error updating branch profile", ex);
                ModelState.AddModelError("", "An error occurred while updating the branch.");
                return View(viewModel);
            }
        }

        // ============================================================
        // ACTIVATE / DEACTIVATE
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "BranchActivate")]
        public async Task<IActionResult> Activate(int id)
        {
            try
            {
                var command = new ActivateBranchCommand { BranchId = id };
                var response = await _mediator.Send(command);
                TempData["SuccessMessage"] = $"Branch '{response.BranchName}' activated successfully.";

                return RedirectToAction(nameof(Profile), new { id });
            }
            catch (Domain.Exceptions.BranchNotFoundException)
            {
                return NotFound();
            }
            catch (Domain.Exceptions.BranchActivationException ex)
            {
                TempData["ErrorMessage"] = ex.Message?.ToString();
                return RedirectToAction(nameof(Profile), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error activating branch {id}", ex);
                TempData["ErrorMessage"] = "An error occurred while activating the branch.";
                return RedirectToAction(nameof(Profile), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "BranchDeactivate")]
        public async Task<IActionResult> Deactivate(int id, string? reason)
        {
            try
            {
                var command = new DeactivateBranchCommand
                {
                    BranchId = id,
                    Reason = reason
                };
                var response = await _mediator.Send(command);
                TempData["SuccessMessage"] = $"Branch '{response.BranchName}' deactivated successfully.";

                return RedirectToAction(nameof(Profile), new { id });
            }
            catch (Domain.Exceptions.BranchNotFoundException)
            {
                return NotFound();
            }
            catch (Domain.Exceptions.BranchDeactivationException ex)
            {
                TempData["ErrorMessage"] = ex.Message?.ToString();
                return RedirectToAction(nameof(Profile), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deactivating branch {id}", ex);
                TempData["ErrorMessage"] = "An error occurred while deactivating the branch.";
                return RedirectToAction(nameof(Profile), new { id });
            }
        }

        // ============================================================
        // SEARCH
        // ============================================================
        [HttpGet]
        [Authorize(Policy = "BranchView")]
        public IActionResult Search()
        {
            return View(new BranchSearchViewModel());
        }

        [HttpPost]
        [Authorize(Policy = "BranchView")]
        public async Task<IActionResult> Search(BranchSearchViewModel viewModel)
        {
            try
            {
                var query = new SearchBranchesQuery
                {
                    SearchTerm = viewModel.SearchTerm,
                    CompanyId = viewModel.CompanyId,
                    IsActive = viewModel.IsActive,
                    Page = viewModel.Page,
                    PageSize = viewModel.PageSize
                };

                var response = await _mediator.Send(query);

                // âœ… FIX: BranchSummaryResponse â†’ BranchViewModel mapping
                viewModel.Branches = response.Branches?
                    .Select(b => new BranchViewModel
                    {
                        BranchId = b.BranchId,
                        BranchName = b.BranchName,
                        BranchCode = b.BranchCode ?? string.Empty,
                        CompanyId = b.CompanyId,
                        CompanyName = b.CompanyName,
                        IsActive = b.IsActive,
                        CreatedDate = b.CreatedDate,
                        EmployeeCount = b.EmployeeCount,
                        WarehouseCount = b.WarehouseCount
                    })
                    .ToList() ?? new List<BranchViewModel>();

                viewModel.TotalCount = response.TotalCount;
                viewModel.TotalPages = response.TotalPages;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error searching branches", ex);
                ModelState.AddModelError("", "An error occurred while searching branches.");
                return View(viewModel);
            }
        }

        // ============================================================
        // VALIDATION ENDPOINTS
        // ============================================================
        [HttpGet]
        [Authorize(Policy = "BranchView")]
        public async Task<IActionResult> ValidateName(string name, int companyId, int? excludeId)
        {
            try
            {
                var isUnique = await _mediator.Send(new ValidateBranchNameRequest
                {
                    BranchName = name,
                    CompanyId = companyId,
                    ExcludingBranchId = excludeId
                });

                return Json(new { valid = isUnique.IsValid });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error validating branch name", ex);
                return Json(new { valid = false, message = "Validation error occurred." });
            }
        }

        [HttpGet]
        [Authorize(Policy = "BranchView")]
        public async Task<IActionResult> ValidateCode(string code, int companyId, int? excludeId)
        {
            try
            {
                var isUnique = await _mediator.Send(new ValidateBranchCodeRequest
                {
                    BranchCode = code,
                    CompanyId = companyId,
                    ExcludingBranchId = excludeId
                });

                return Json(new { valid = isUnique.IsValid });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error validating branch code", ex);
                return Json(new { valid = false, message = "Validation error occurred." });
            }
        }

        // ============================================================
        // DROPDOWN ENDPOINTS
        // ============================================================
        [HttpGet]
        [Authorize(Policy = "BranchView")]
        public async Task<IActionResult> GetBranchesByCompany(int companyId, bool? isActive)
        {
            try
            {
                var query = new GetBranchesByCompanyQuery
                {
                    CompanyId = companyId,
                    IsActive = isActive
                };
                var branches = await _mediator.Send(query);
                return Ok(branches);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting branches for company {companyId}", ex);
                return BadRequest(new { error = "An error occurred while fetching branches." });
            }
        }

        [HttpGet]
        [Authorize(Policy = "BranchView")]
        public async Task<IActionResult> GetActiveBranches()
        {
            try
            {
                var branches = await _context.Branches
                    .Where(b => b.IsActive == true && b.IsDeleted == false)
                    .Select(b => new
                    {
                        branchId = b.BranchId,
                        branchName = b.BranchName,
                        branchCode = b.BranchCode
                    })
                    .OrderBy(b => b.branchName)
                    .ToListAsync();

                return Ok(branches);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting active branches", ex);
                return BadRequest(new { error = "Failed to load branches." });
            }
        }

        [HttpGet]
        [Authorize(Policy = "CompanyView")]
        public async Task<IActionResult> GetCompaniesForDropdown()
        {
            try
            {
                var companies = await _context.Companies
                    .Where(c => c.IsActive == true && c.IsDeleted == false)
                    .Select(c => new
                    {
                        companyId = c.CompanyId,
                        companyName = c.CompanyName
                    })
                    .OrderBy(c => c.companyName)
                    .ToListAsync();

                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting companies for dropdown", ex);
                return BadRequest(new { error = "Failed to load companies." });
            }
        }

        // ============================================================
        // STATISTICS
        // ============================================================
        [HttpGet]
        [Authorize(Policy = "BranchView")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var total = await _context.Branches.CountAsync(b => b.IsDeleted != true);
                var active = await _context.Branches.CountAsync(b => b.IsActive == true && b.IsDeleted != true);
                var inactive = await _context.Branches.CountAsync(b => b.IsActive == false && b.IsDeleted != true);

                var now = DateTime.Now;
                var startOfMonth = new DateTime(now.Year, now.Month, 1);
                var addedThisMonth = await _context.Branches
                    .CountAsync(b => b.CreatedDate >= startOfMonth && b.IsDeleted != true);

                return Json(new
                {
                    total = total,
                    active = active,
                    inactive = inactive,
                    addedThisMonth = addedThisMonth
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting branch statistics", ex);
                return Json(new { total = 0, active = 0, inactive = 0, addedThisMonth = 0 });
            }
        }

        // ============================================================
        // GET ALL BRANCHES (for Table)
        // ============================================================
        [HttpGet]
        [Authorize(Policy = "BranchView")]
        public async Task<IActionResult> GetAllBranches()
        {
            try
            {
                var branches = await _context.Branches
                    .Include(b => b.Company)
                    .Where(b => b.IsDeleted != true)
                    .OrderBy(b => b.CreatedDate)
                    .Select(b => new
                    {
                        branchId = b.BranchId,
                        branchName = b.BranchName,
                        branchCode = b.BranchCode,
                        companyName = b.Company != null ? b.Company.CompanyName : "N/A",
                        isActive = b.IsActive,
                        createdDate = b.CreatedDate
                    })
                    .ToListAsync();

                return Ok(branches);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting all branches", ex);
                return BadRequest(new { error = "Failed to load branches." });
            }
        }

        // ============================================================
        // AJAX PARTIALS
        // ============================================================
        [HttpGet]
        [Authorize(Policy = "BranchCreate")]
        public IActionResult AddBranchPartial()
        {
            var model = new RegisterBranchViewModel();
            return PartialView("_AddBranchForm", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "BranchCreate")]
        public async Task<IActionResult> CreateBranchAjax(RegisterBranchViewModel viewModel)
        {
            _logger.LogInformation($"Creating branch: {viewModel.BranchName}");

            if (!ModelState.IsValid)
            {
                var html = await RenderPartialViewToString("_AddBranchForm", viewModel);
                return Json(new { success = false, html = html, message = "Please check the form fields." });
            }

            try
            {
                var request = new RegisterBranchRequest
                {
                    BranchName = viewModel.BranchName,
                    BranchCode = viewModel.BranchCode,
                    Address = viewModel.Address,
                    Phone = viewModel.Phone,
                    Email = viewModel.Email,
                    CompanyId = viewModel.CompanyId ?? 0          // âœ… FIX
                };

                var response = await _mediator.Send(request);

                return Json(new
                {
                    success = true,
                    message = $"Branch '{response.BranchName}' registered successfully!",
                    branchId = response.BranchId
                });
            }
            catch (Domain.Exceptions.BranchAlreadyExistsException ex)
            {
                ModelState.AddModelError("BranchName", ex.Message);
                var html = await RenderPartialViewToString("_AddBranchForm", viewModel);
                return Json(new { success = false, html = html, message = "Operation failed." });
            }
            catch (Domain.Exceptions.CompanyNotFoundExceptionForBranch ex)
            {
                ModelState.AddModelError("CompanyId", ex.Message);
                var html = await RenderPartialViewToString("_AddBranchForm", viewModel);
                return Json(new { success = false, html = html, message = "Operation failed." });
            }
            catch (Domain.Exceptions.CompanyInactiveException ex)
            {
                ModelState.AddModelError("CompanyId", ex.Message);
                var html = await RenderPartialViewToString("_AddBranchForm", viewModel);
                return Json(new { success = false, html = html, message = "Operation failed." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating branch via AJAX: {ex.Message}", ex);
                ModelState.AddModelError("", "An error occurred while registering the branch.");
                var html = await RenderPartialViewToString("_AddBranchForm", viewModel);
                return Json(new { success = false, html = html, message = "An error occurred." });
            }
        }

        [HttpGet]
        [Authorize(Policy = "BranchUpdate")]
        public async Task<IActionResult> EditBranchPartial(int id)
        {
            try
            {
                var query = new GetBranchProfileQuery { BranchId = id };
                var profile = await _mediator.Send(query);

                var viewModel = new RegisterBranchViewModel
                {
                    BranchId = profile.BranchId,
                    BranchName = profile.BranchName,
                    BranchCode = profile.BranchCode ?? string.Empty,
                    Address = profile.Address,
                    Phone = profile.Phone,
                    Email = profile.Email,
                    CompanyId = profile.CompanyId,
                    CompanyName = profile.CompanyName,
                    IsActive = profile.IsActive
                };

                return PartialView("_EditBranchForm", viewModel);
            }
            catch (Domain.Exceptions.BranchNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading edit form for branch {id}", ex);
                return BadRequest();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "BranchUpdate")]
        public async Task<IActionResult> UpdateBranchAjax(RegisterBranchViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var html = await RenderPartialViewToString("_EditBranchForm", viewModel);
                return Json(new { success = false, html = html, message = "Please check the form fields." });
            }

            try
            {
                var request = new ChangeBranchProfileRequest
                {
                    BranchId = viewModel.BranchId ?? 0,          // âœ… FIX
                    BranchName = viewModel.BranchName,
                    BranchCode = viewModel.BranchCode,
                    Address = viewModel.Address,
                    Phone = viewModel.Phone,
                    Email = viewModel.Email
                };

                var response = await _mediator.Send(request);

                return Json(new
                {
                    success = true,
                    message = $"Branch '{response.BranchName}' updated successfully!",
                    branchId = response.BranchId
                });
            }
            catch (Domain.Exceptions.BranchNotFoundException)
            {
                return Json(new { success = false, message = "Branch not found." });
            }
            catch (Domain.Exceptions.BranchAlreadyExistsException ex)
            {
                ModelState.AddModelError("BranchName", ex.Message);
                var html = await RenderPartialViewToString("_EditBranchForm", viewModel);
                return Json(new { success = false, html = html, message = "Operation failed." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating branch via AJAX: {ex.Message}", ex);
                ModelState.AddModelError("", "An error occurred while updating the branch.");
                var html = await RenderPartialViewToString("_EditBranchForm", viewModel);
                return Json(new { success = false, html = html, message = "An error occurred." });
            }
        }

        // ============================================================
        // HELPER: Render Partial to String
        // ============================================================
        private async Task<string> RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = ControllerContext.ActionDescriptor.ActionName;
            }

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewEngine = HttpContext.RequestServices.GetService(typeof(IRazorViewEngine)) as IRazorViewEngine;

                if (viewEngine == null)
                {
                    throw new InvalidOperationException("IRazorViewEngine service not found.");
                }

                var viewResult = viewEngine.FindView(ControllerContext, viewName, false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException($"{viewName} does not match any available view");
                }

                var viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    sw,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return sw.GetStringBuilder().ToString();
            }
        }

        // ============================================================
        // DELETE (Soft)
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "BranchDelete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                if (id <= 0)
                    return Json(new { success = false, message = "Invalid ID." });

                var branch = await _context.Branches.FindAsync(id);
                if (branch == null || branch.IsDeleted == true)
                    return Json(new { success = false, message = "Branch not found." });

                branch.IsDeleted = true;
                branch.IsActive = false;
                branch.DeletedDate = DateTime.Now;

                var uidClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(uidClaim, out int uid))
                    branch.DeletedBy = uid.ToString();

                var userName = User.Identity?.Name ?? "Unknown";
                var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "";
                branch.DeletedByName = string.IsNullOrEmpty(userRole)
                    ? userName
                    : $"{userName} ({userRole})";

                await _context.SaveChangesAsync();
                _logger.LogInformation($"Branch soft-deleted: {branch.BranchName} (ID: {id})");

                return Json(new { success = true, message = $"Branch '{branch.BranchName}' deleted successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting branch {id}", ex);
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }

        // ============================================================
        // TOGGLE STATUS
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Manage")]
        public async Task<IActionResult> ToggleBranchStatus(int id)
        {
            try
            {
                var b = await _context.Branches.FindAsync(id);
                if (b == null) return Json(new { success = false, message = "Branch not found" });
                b.IsActive = !(b.IsActive ?? false);
                b.UpdatedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = b.IsActive == true ? "Branch Activated" : "Branch Deactivated" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException != null ? ex.InnerException.Message : ex.Message });
            }
        }

        // ============================================================
        // DELETE BRANCH JSON
        // ============================================================
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBranchJson(int id)
        {
            try
            {
                var b = await _context.Branches.FindAsync(id);
                if (b == null) return Json(new { success = false, message = "Branch not found" });
                b.IsActive = false;
                b.IsDeleted = true;
                b.DeletedDate = DateTime.Now;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Branch Deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException != null ? ex.InnerException.Message : ex.Message });
            }
        }
    }
}
