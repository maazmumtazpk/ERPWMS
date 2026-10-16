
    using ERPWMS.Domain.Interfaces;
    using ERPWMS.Domain.Services;
    using System;
    using System.Threading.Tasks;
using ERPWMS.Database.Models;

    namespace ERPWMS.Infrastructure.Services
    {
        public class ChartOfAccountBusinessRules : IChartOfAccountBusinessRules
        {
            private readonly IChartOfAccountRepository _repository;

            public ChartOfAccountBusinessRules(IChartOfAccountRepository repository)
            {
                _repository = repository;
            }

            public async Task ValidateParentAccountAsync(int companyId, int? parentAccountId)
            {
                if (!parentAccountId.HasValue)
                    return;

                var parent = await _repository.GetByIdAsync(parentAccountId.Value, companyId);
                if (parent == null)
                    throw new InvalidOperationException("Parent account does not exist.");

                if (parent.IsDeleted == true)
                    throw new InvalidOperationException("Parent account is deleted and cannot be used.");

                if (parent.IsActive != true)
                    throw new InvalidOperationException("Parent account is inactive and cannot be used.");
            }

            public async Task ValidateDeleteAsync(int accountId, int companyId)
            {
                var account = await _repository.GetByIdAsync(accountId, companyId);
                if (account == null)
                    throw new InvalidOperationException("Account not found.");

                if (account.IsDeleted == true)
                    throw new InvalidOperationException("Account is already deleted.");

                var hasChildren = await _repository.HasChildrenAsync(accountId, companyId);
                if (hasChildren)
                    throw new InvalidOperationException("Account has child accounts. Cannot delete.");
            }

            public async Task ValidateCodeUniquenessAsync(int companyId, string accountCode, int? excludeId = null)
            {
                var exists = await _repository.IsCodeExistsAsync(companyId, accountCode, excludeId);
                if (exists)
                    throw new InvalidOperationException("Account Code already exists in this company.");
            }

            public Task ValidateAccountTypeAsync(string accountType)
            {
                var validTypes = new[] { "Asset", "Liability", "Equity", "Income", "Expense" };
                if (string.IsNullOrEmpty(accountType) || !validTypes.Contains(accountType))
                    throw new InvalidOperationException("Invalid Account Type. Allowed: Asset, Liability, Equity, Income, Expense");

                return Task.CompletedTask;
            }
        }
    }



