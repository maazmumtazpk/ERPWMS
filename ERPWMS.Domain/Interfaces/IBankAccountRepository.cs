
using ERPWMS.Database.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Domain.Interfaces
{
    public interface IBankAccountRepository
    {
        // ✅ All methods include companyId for filtering
        Task<List<BankAccount>> GetAllAsync(int companyId, bool includeDeleted = false);
        Task<BankAccount?> GetByIdAsync(int id, int companyId);
        Task<BankAccount?> GetByAccountNoAsync(int companyId, string accountNo);
        Task<bool> IsAccountNoExistsAsync(int companyId, string accountNo, int? excludeId = null);
        Task<bool> IsIbanExistsAsync(int companyId, string iban, int? excludeId = null);
        Task<bool> CanDeleteAsync(int accountId, int companyId);
        Task AddAsync(BankAccount entity);
        Task UpdateAsync(BankAccount entity);
        Task SoftDeleteAsync(int id, int companyId, string deletedBy);
    }
}




