using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using ERPWMS.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services.Notification
{
    public class NotificationTemplateService : INotificationTemplateService
    {
        private readonly AppDbContext _context;

        // =========================================================
        // APPROVED TEMPLATE VARIABLES
        // =========================================================
        private static readonly string[] ApprovedVariables =
        {
            "{{UserName}}",
            "{{UserEmail}}",
            "{{UserFirstName}}",
            "{{UserLastName}}",
            "{{ReferenceNo}}",
            "{{ReferenceModule}}",
            "{{NotificationSubject}}",
            "{{NotificationMessage}}",
            "{{CompanyName}}",
            "{{CompanyAddress}}",
            "{{CompanyPhone}}",
            "{{CurrentDate}}",
            "{{CurrentTime}}",
            "{{SupportEmail}}",
            "{{ApplicationUrl}}",
            "{{PasswordResetLink}}",
            "{{ExpiryDate}}",
            "{{ActionUrl}}",
            "{{Comment}}"
        };

        // =========================================================
        // CONSTRUCTOR
        // =========================================================
        public NotificationTemplateService(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET ALL TEMPLATES
        // =========================================================
        public async Task<IEnumerable<NotificationTemplate>> GetAllTemplatesAsync(bool includeInactive = false)
        {
            // ✅ FIX:  hata diya
            // (kyunke ye [NotMapped] hai, EF ise load nahi kar sakta)

            IQueryable<NotificationTemplate> query = _context.NotificationTemplates;

            if (!includeInactive)
            {
                query = query.Where(t => t.IsActive == true);
            }

            return await query
                .OrderBy(t => t.TemplateCode)
                .ToListAsync();
        }

        // =========================================================
        // GET TEMPLATE BY ID
        // =========================================================
        public async Task<NotificationTemplate?> GetTemplateByIdAsync(int templateId)
        {
            return await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.TemplateId == templateId);
        }

        // =========================================================
        // GET TEMPLATE BY CODE
        // =========================================================
        public async Task<NotificationTemplate?> GetTemplateByCodeAsync(string templateCode)
        {
            if (string.IsNullOrWhiteSpace(templateCode))
            {
                return null;
            }

            return await _context.NotificationTemplates
                .FirstOrDefaultAsync(t => t.TemplateCode == templateCode);
        }

        // =========================================================
        // CREATE TEMPLATE
        // =========================================================
        public async Task<NotificationTemplate> CreateTemplateAsync(NotificationTemplate template)
        {
            if (template is null)   // ✅ simplified
            {
                throw new ArgumentNullException(nameof(template));
            }

            template.CreatedDate = DateTime.UtcNow;
            template.IsActive = true;

            _context.NotificationTemplates.Add(template);
            await _context.SaveChangesAsync();

            return template;
        }

        // =========================================================
        // UPDATE TEMPLATE
        // =========================================================
        public async Task UpdateTemplateAsync(NotificationTemplate template)
        {
            if (template is null)   // ✅ simplified
            {
                throw new ArgumentNullException(nameof(template));
            }

            template.UpdatedDate = DateTime.UtcNow;

            _context.NotificationTemplates.Update(template);
            await _context.SaveChangesAsync();
        }

        // =========================================================
        // DELETE TEMPLATE
        // =========================================================
        public async Task<bool> DeleteTemplateAsync(int templateId)
        {
            var template = await GetTemplateByIdAsync(templateId);

            if (template is null)   // ✅ simplified
            {
                return false;
            }

            // -----------------------------------------------------
            // CHECK EXISTING NOTIFICATIONS
            // -----------------------------------------------------
            var hasNotifications = await _context.Notifications
                .AnyAsync(n => n.TemplateId == templateId);

            if (hasNotifications)
            {
                throw new InvalidOperationException(
                    "Cannot delete template because it is " +
                    "used by existing notifications.");
            }

            // -----------------------------------------------------
            // DELETE
            // -----------------------------------------------------
            _context.NotificationTemplates.Remove(template);
            await _context.SaveChangesAsync();

            return true;
        }

        // =========================================================
        // TEMPLATE CODE EXISTS
        // =========================================================
        public async Task<bool> TemplateCodeExistsAsync(string templateCode, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(templateCode))
            {
                return false;
            }

            IQueryable<NotificationTemplate> query = _context.NotificationTemplates
                .Where(t => t.TemplateCode == templateCode);

            if (excludeId.HasValue)
            {
                query = query.Where(t => t.TemplateId != excludeId);
            }

            return await query.AnyAsync();
        }

        // =========================================================
        // CHECK TEMPLATE ACTIVE
        // =========================================================
        public async Task<bool> IsTemplateActiveAsync(int templateId)
        {
            var template = await GetTemplateByIdAsync(templateId);

            return template != null && template.IsActive == true;
        }

        // =========================================================
        // PROCESS TEMPLATE
        // =========================================================
        // Return format: Subject|||Body
        // =========================================================
        public async Task<string> ProcessTemplateAsync(
            NotificationTemplate template,
            Dictionary<string, string> variables)
        {
            if (template is null)
            {
                throw new ArgumentNullException(nameof(template));
            }

            if (variables is null)
            {
                throw new ArgumentNullException(nameof(variables));
            }

            var content = template.Body ?? string.Empty;
            var subject = template.Subject ?? string.Empty;

            var processedSubject = ReplaceVariables(subject, variables);
            var processedContent = ReplaceVariables(content, variables);

            return processedSubject + "|||" + processedContent;
        }

        // =========================================================
        // REPLACE VARIABLES
        // =========================================================
        private static string ReplaceVariables(
            string content,
            Dictionary<string, string> variables)
        {
            if (string.IsNullOrEmpty(content))
            {
                return content;
            }

            var result = content;

            // -----------------------------------------------------
            // REPLACE APPROVED VARIABLES
            // -----------------------------------------------------
            foreach (var variable in ApprovedVariables)
            {
                var key = variable
                    .Replace("{{", string.Empty)
                    .Replace("}}", string.Empty);

                if (variables.TryGetValue(key, out var value))
                {
                    result = result.Replace(variable, value ?? string.Empty);
                }
            }

            // -----------------------------------------------------
            // REMOVE UNAPPROVED VARIABLES
            // -----------------------------------------------------
            const string unapprovedPattern = @"\{\{[^}]+\}\}";

            result = Regex.Replace(result, unapprovedPattern, string.Empty);

            return result;
        }

        // =========================================================
        // VALIDATE TEMPLATE VARIABLES
        // =========================================================
        public async Task<bool> ValidateTemplateVariablesAsync(
            string body,
            string[] requiredVariables)
        {
            if (string.IsNullOrEmpty(body))
            {
                return false;
            }

            if (requiredVariables is null)
            {
                return true;
            }

            // -----------------------------------------------------
            // REQUIRED VARIABLES
            // -----------------------------------------------------
            foreach (var required in requiredVariables)
            {
                if (string.IsNullOrWhiteSpace(required))
                {
                    continue;
                }

                var variablePattern = $"{{{{{required}}}}}";

                if (!body.Contains(variablePattern, StringComparison.Ordinal))
                {
                    return false;
                }
            }

            // -----------------------------------------------------
            // FIND ALL VARIABLES
            // -----------------------------------------------------
            var allVariables = Regex.Matches(body, @"\{\{[^}]+\}\}")
                .Select(match => match.Value)
                .Distinct(StringComparer.Ordinal);

            // -----------------------------------------------------
            // CHECK APPROVED VARIABLES
            // -----------------------------------------------------
            foreach (var variable in allVariables)
            {
                if (!ApprovedVariables.Contains(variable, StringComparer.Ordinal))
                {
                    return false;
                }
            }

            return true;
        }
    }
}