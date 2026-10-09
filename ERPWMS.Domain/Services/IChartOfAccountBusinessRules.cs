
    using System.Threading.Tasks;

    namespace ERPWMS.Domain.Services
    {
        public interface IChartOfAccountBusinessRules
        {
            Task ValidateParentAccountAsync(int companyId, int? parentAccountId);
            Task ValidateDeleteAsync(int accountId, int companyId);
            Task ValidateCodeUniquenessAsync(int companyId, string accountCode, int? excludeId = null);
            Task ValidateAccountTypeAsync(string accountType);
        }
    }
