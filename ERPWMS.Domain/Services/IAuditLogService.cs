using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Services
{
    public interface IAuditLogService
    {
        Task<IEnumerable<AuditLog>> GetAuditLogsAsync(
            int? userId = null,
            int? moduleId = null,
            string? action = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            string? recordType = null,
            int? recordId = null,
            int page = 1,
            int pageSize = 50);

        Task<int> GetAuditLogCountAsync(
            int? userId = null,
            int? moduleId = null,
            string? action = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        Task<AuditLog?> GetAuditLogByIdAsync(int auditId);
        Task<IEnumerable<string>> GetDistinctActionsAsync();
        Task<IEnumerable<string>> GetDistinctRecordTypesAsync();
    }
}