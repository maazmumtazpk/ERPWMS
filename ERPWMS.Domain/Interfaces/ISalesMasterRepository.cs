using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface ISalesMasterRepository
    {
        Task<IEnumerable<SalesMaster>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<SalesMaster?> GetByIdAsync(int salesId, int companyId);
        Task<SalesMaster?> GetByOrderNoAsync(string salesOrderNo, int companyId);
        Task<IEnumerable<SalesMaster>> GetByCustomerIdAsync(int customerId, int companyId);
        Task<IEnumerable<SalesMaster>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId);
        Task<IEnumerable<SalesMaster>> GetByStatusAsync(string status, int companyId);
        Task<bool> IsOrderNoUniqueAsync(string salesOrderNo, int companyId, int? excludeId = null);
        Task AddAsync(SalesMaster entity);
        Task UpdateAsync(SalesMaster entity);
        Task SoftDeleteAsync(int salesId, int companyId, string deletedBy);
        Task<bool> CanBeDeletedAsync(int salesId, int companyId);
        Task UpdateStatusAsync(int salesId, string status, int? workflowInstanceId = null, int? currentStageId = null);
    }
}



