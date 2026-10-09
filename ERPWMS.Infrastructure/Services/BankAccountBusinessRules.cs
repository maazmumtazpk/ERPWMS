
using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using System;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Infrastructure.Services
{
    public class BankAccountBusinessRules : IBankAccountBusinessRules
    {
        private readonly IBankAccountRepository _repository;

        public BankAccountBusinessRules(IBankAccountRepository repository)
        {
            _repository = repository;
        }

        public async Task ValidateDuplicateAccountNoAsync(int companyId, string accountNo, int? excludeId = null)
        {
            var exists = await _repository.IsAccountNoExistsAsync(companyId, accountNo, excludeId);
            if (exists)
                throw new Exception("Account Number already exists in this company.");
        }

        public async Task ValidateDuplicateIbanAsync(int companyId, string iban, int? excludeId = null)
        {
            if (string.IsNullOrEmpty(iban)) return;

            var exists = await _repository.IsIbanExistsAsync(companyId, iban, excludeId);
            if (exists)
                throw new Exception("IBAN already exists in this company.");
        }

        public async Task ValidateDeleteAsync(int accountId, int companyId)
        {
            var canDelete = await _repository.CanDeleteAsync(accountId, companyId);
            if (!canDelete)
                throw new Exception("Cannot delete bank account. It may have related transactions.");
        }

        public Task ValidateOpeningBalanceAsync(decimal openingBalance)
        {
            if (openingBalance < 0)
                throw new Exception("Opening balance cannot be negative.");
            return Task.CompletedTask;
        }

        public Task ValidateAccountTypeAsync(string? accountType)
        {
            if (!string.IsNullOrEmpty(accountType))
            {
                var validTypes = new[] { "Current", "Saving", "Fixed", "Others" };
                if (!validTypes.Contains(accountType))
                    throw new Exception("Invalid Account Type. Allowed: Current, Saving, Fixed, Others");
            }
            return Task.CompletedTask;
        }
    }
}

