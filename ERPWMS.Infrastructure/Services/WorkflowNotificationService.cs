using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services.Workflow
{
    public class WorkflowNotificationService
    {
        private readonly INotificationService _notificationService;
        private readonly INotificationTemplateService _templateService;

        public WorkflowNotificationService(INotificationService notificationService,
                                            INotificationTemplateService templateService)
        {
            _notificationService = notificationService;
            _templateService = templateService;
        }

        public async Task SendWorkflowNotificationAsync(int userId, string templateCode, Dictionary<string, string> variables,
                                                         string? referenceNo = null, string? referenceModule = "Workflow")
        {
            var template = await _templateService.GetTemplateByCodeAsync(templateCode);
            if (template == null || template.IsActive != true)
                return; // Skip if template inactive

            // Process template with variables
            var subject = template.Subject;
            var body = template.Body;
            // Replace variables (basic)
            foreach (var kv in variables)
            {
                subject = subject.Replace($"{{{{{kv.Key}}}}}", kv.Value);
                body = body.Replace($"{{{{{kv.Key}}}}}", kv.Value);
            }

            await _notificationService.CreateNotificationAsync(
                userId,
                template.TemplateId,
                subject,
                body,
                type: "Workflow",
                referenceNo: referenceNo,
                referenceModule: referenceModule
            );
        }
    }
}