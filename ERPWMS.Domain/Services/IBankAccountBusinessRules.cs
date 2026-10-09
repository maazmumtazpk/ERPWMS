
using System.Threading.Tasks;
namespace ERPWMS.Domain.Services
{ 
    public interface IBankAccountBusinessRules
    {
        Task ValidateDuplicateAccountNoAsync(int companyId, string accountNo, int? excludeId = null);
        Task ValidateDuplicateIbanAsync(int companyId, string iban, int? excludeId = null);
        Task ValidateDeleteAsync(int accountId, int companyId);
        Task ValidateOpeningBalanceAsync(decimal openingBalance);
        Task ValidateAccountTypeAsync(string? accountType);
    }
}
