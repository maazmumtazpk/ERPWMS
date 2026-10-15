using ERPWMS.Application.DTOs.Notification;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Controllers.Admin
{
    [Authorize(Policy = "SystemAdmin.View")]
    public class NotificationTemplateController : Controller
    {
        private readonly INotificationTemplateService _templateService;

        public NotificationTemplateController(INotificationTemplateService templateService)
        {
            _templateService = templateService;
        }

        public async Task<IActionResult> Index(string searchTerm, bool? isActive)
        {
            var templates = await _templateService.GetAllTemplatesAsync(includeInactive: true);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var search = searchTerm.ToLower().Trim();
                templates = templates.Where(t =>
                    t.TemplateName.ToLower().Contains(search) ||
                    t.TemplateCode.ToLower().Contains(search) ||
                    (t.Type != null && t.Type.ToLower().Contains(search))
                );
            }

            if (isActive.HasValue)
            {
                templates = templates.Where(t => t.IsActive == isActive);
            }

            var dtos = templates.Select(t => new NotificationTemplateDto
            {
                TemplateId = t.TemplateId,
                TemplateName = t.TemplateName,
                TemplateCode = t.TemplateCode,
                Subject = t.Subject,
                Body = t.Body,
                Type = t.Type,
                IsActive = t.IsActive,
                CreatedDate = t.CreatedDate,
                CreatedBy = t.CreatedByNavigation?.UserName,
                UpdatedDate = t.UpdatedDate,
                UpdatedBy = t.UpdatedByNavigation?.UserName
            });

            return View(dtos);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Create")]
        public async Task<IActionResult> Create(CreateNotificationTemplateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var template = new NotificationTemplate
            {
                TemplateName = request.TemplateName,
                TemplateCode = request.TemplateCode,
                Subject = request.Subject,
                Body = request.Body,
                Type = request.Type,
                IsActive = request.IsActive,
                CreatedBy = request.CreatedBy,
                CreatedDate = System.DateTime.UtcNow
            };

            await _templateService.CreateTemplateAsync(template);
            TempData["Success"] = "Notification template created successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var template = await _templateService.GetTemplateByIdAsync(id);
            if (template == null)
                return NotFound();

            var model = new UpdateNotificationTemplateRequest
            {
                TemplateId = template.TemplateId,
                TemplateName = template.TemplateName,
                TemplateCode = template.TemplateCode,
                Subject = template.Subject,
                Body = template.Body,
                Type = template.Type,
                IsActive = template.IsActive,
                UpdatedBy = 0 // Will be set in POST
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Edit")]
        public async Task<IActionResult> Edit(UpdateNotificationTemplateRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var template = await _templateService.GetTemplateByIdAsync(request.TemplateId);
            if (template == null)
                return NotFound();

            template.TemplateName = request.TemplateName;
            template.TemplateCode = request.TemplateCode;
            template.Subject = request.Subject;
            template.Body = request.Body;
            template.Type = request.Type;
            template.IsActive = request.IsActive ?? template.IsActive;
            template.UpdatedBy = request.UpdatedBy.ToString();
            template.UpdatedDate = System.DateTime.UtcNow;

            await _templateService.UpdateTemplateAsync(template);
            TempData["Success"] = "Notification template updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "SystemAdmin.Delete")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _templateService.DeleteTemplateAsync(id);
                TempData["Success"] = "Template deleted successfully.";
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = ex.Message?.ToString();
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Preview(int id)
        {
            var template = await _templateService.GetTemplateByIdAsync(id);
            if (template == null)
                return NotFound();

            var sampleVariables = new Dictionary<string, string>
            {
                ["UserName"] = "John Doe",
                ["UserEmail"] = "john@example.com",
                ["UserFirstName"] = "John",
                ["UserLastName"] = "Doe",
                ["ReferenceNo"] = "REF-001",
                ["ReferenceModule"] = "Test",
                ["CompanyName"] = "EnterpriseWMS",
                ["CurrentDate"] = System.DateTime.Now.ToString("dd-MMM-yyyy"),
                ["CurrentTime"] = System.DateTime.Now.ToString("HH:mm"),
                ["ApplicationUrl"] = "https://localhost:5001"
            };

            var processed = await _templateService.ProcessTemplateAsync(template, sampleVariables);
            var parts = processed.Split("|||");
            var subject = parts.Length > 0 ? parts[0] : template.Subject;
            var body = parts.Length > 1 ? parts[1] : template.Body;

            ViewBag.Subject = subject;
            ViewBag.Body = body;
            ViewBag.TemplateName = template.TemplateName;
            return View();
        }
    }
}
