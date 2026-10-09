using MediatR;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions;
using ERPWMS.Database;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Commands.EmployeeBankAccount.ChangeEmployeeBankAccount
{
    public class ChangeEmployeeBankAccountCommandHandler
        : IRequestHandler<
            ChangeEmployeeBankAccountCommand,
            ChangeEmployeeBankAccountResponse>
    {
        private readonly AppDbContext _context;

        public ChangeEmployeeBankAccountCommandHandler(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<ChangeEmployeeBankAccountResponse> Handle(
            ChangeEmployeeBankAccountCommand request,
            CancellationToken cancellationToken)
        {
            // =====================================================
            // FIND BANK ACCOUNT
            // =====================================================

            var entity = await _context.EmployeeBankAccounts
                .FirstOrDefaultAsync(
                    x =>
                        x.BankAccountId == request.BankAccountId &&
                        x.EmployeeId == request.EmployeeId,
                    cancellationToken);

            if (entity == null)
            {
                // ✅ FIX: int → string
                throw new EmployeeBankAccountNotFoundException(
                    request.BankAccountId.ToString());
            }

            // =====================================================
            // CHECK ACTIVE
            // =====================================================

            if (entity.IsActive != true)
            {
                // ✅ FIX: int → string
                throw new EmployeeBankAccountInactiveException(
                    entity.BankAccountId.ToString());
            }

            var dto = request.Request;

            // =====================================================
            // VALIDATION
            // =====================================================

            if (string.IsNullOrWhiteSpace(dto.BankName))
            {
                throw new EmployeeBankAccountValidationException(
                    "Bank name is required.");
            }

            if (string.IsNullOrWhiteSpace(dto.AccountNumber))
            {
                throw new EmployeeBankAccountValidationException(
                    "Account number is required.");
            }

            // =====================================================
            // DUPLICATE ACCOUNT CHECK
            // =====================================================

            var duplicate = await _context.EmployeeBankAccounts
                .AnyAsync(
                    x =>
                        x.BankAccountId != entity.BankAccountId &&
                        x.EmployeeId == request.EmployeeId &&
                        x.AccountNumber == dto.AccountNumber,
                    cancellationToken);

            if (duplicate)
            {
                throw new EmployeeBankAccountValidationException(
                    "Another bank account with this account number already exists.");
            }

            // =====================================================
            // PRIMARY ACCOUNT
            // =====================================================

            if (dto.IsPrimary == true)
            {
                var primaryAccounts =
                    await _context.EmployeeBankAccounts
                        .Where(x =>
                            x.EmployeeId == request.EmployeeId &&
                            x.BankAccountId != entity.BankAccountId &&
                            x.IsPrimary == true &&
                            x.IsActive == true)
                        .ToListAsync(cancellationToken);

                foreach (var account in primaryAccounts)
                {
                    account.IsPrimary = false;
                    account.UpdatedBy = request.CurrentUserId.ToString();
                    account.UpdatedDate = DateTime.UtcNow;
                }
            }

            // =====================================================
            // UPDATE
            // =====================================================

            entity.BankName = dto.BankName.Trim();

            entity.AccountNumber = dto.AccountNumber.Trim();

            entity.Iban = string.IsNullOrWhiteSpace(dto.Iban)
                ? null
                : dto.Iban.Trim();

            entity.AccountType = string.IsNullOrWhiteSpace(dto.AccountType)
                ? null
                : dto.AccountType.Trim();

            entity.IsPrimary = dto.IsPrimary ?? false;

            if (dto.IsActive.HasValue)
            {
                entity.IsActive = dto.IsActive.Value;
            }

            entity.UpdatedBy = request.CurrentUserId.ToString();
            entity.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // =====================================================
            // RESPONSE
            // =====================================================

            return new ChangeEmployeeBankAccountResponse
            {
                BankAccountId = entity.BankAccountId,
                Message = "Employee bank account updated successfully."
            };
        }
    }
}