
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace ERPWMS.Domain.Interfaces

{
    public interface IChartOfAccountRepository
    {
        // ✅ All methods include companyId for filtering
        Task<List<ChartOfAccount>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<ChartOfAccount?> GetByIdAsync(int id, int companyId);
        Task<ChartOfAccount?> GetByCodeAsync(int companyId, string accountCode);
        Task<bool> IsCodeExistsAsync(int companyId, string accountCode, int? excludeId = null);
        Task<bool> HasChildrenAsync(int accountId, int companyId);
        Task<bool> IsParentValidAsync(int companyId, int? parentId);
        Task AddAsync(ChartOfAccount entity);
        Task UpdateAsync(ChartOfAccount entity);
        Task SoftDeleteAsync(int id, int companyId, string deletedBy);
        Task<bool> CanDeleteAsync(int accountId, int companyId);
    }
}




