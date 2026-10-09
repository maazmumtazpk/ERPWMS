using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface INotificationTemplateService
    {
        // Template management
        Task<IEnumerable<NotificationTemplate>> GetAllTemplatesAsync(bool includeInactive = false);
        Task<NotificationTemplate?> GetTemplateByIdAsync(int templateId);
        Task<NotificationTemplate?> GetTemplateByCodeAsync(string templateCode);
        Task<NotificationTemplate> CreateTemplateAsync(NotificationTemplate template);
        Task UpdateTemplateAsync(NotificationTemplate template);
        Task<bool> DeleteTemplateAsync(int templateId);
        Task<bool> TemplateCodeExistsAsync(string templateCode, int? excludeId = null);
        Task<bool> IsTemplateActiveAsync(int templateId);

        // Template processing
        Task<string> ProcessTemplateAsync(NotificationTemplate template, Dictionary<string, string> variables);
        Task<bool> ValidateTemplateVariablesAsync(string body, string[] requiredVariables);
    }
}