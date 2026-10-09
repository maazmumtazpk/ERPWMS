
using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface ITaxSettingRepository
    {
        // ✅ All methods include companyId for filtering
        Task<List<TaxSetting>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<TaxSetting?> GetByIdAsync(int id, int companyId);
        Task<TaxSetting?> GetByCodeAsync(int companyId, string taxCode);
        Task<TaxSetting?> GetDefaultTaxAsync(int companyId);
        Task<bool> HasDefaultTaxAsync(int companyId, int? excludeId = null);
        Task<bool> IsCodeExistsAsync(int companyId, string taxCode, int? excludeId = null);
        Task AddAsync(TaxSetting entity);
        Task UpdateAsync(TaxSetting entity);
        Task SoftDeleteAsync(int id, int companyId, string deletedBy);
    }
}





