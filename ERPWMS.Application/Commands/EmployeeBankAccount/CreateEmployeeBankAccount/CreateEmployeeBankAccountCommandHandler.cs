using MediatR;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions;
using ERPWMS.Database;

namespace ERPWMS.Application.Commands.EmployeeBankAccount.CreateEmployeeBankAccount
{
    public class CreateEmployeeBankAccountCommandHandler
        : IRequestHandler<
            CreateEmployeeBankAccountCommand,
            CreateEmployeeBankAccountResponse>
    {
        private readonly AppDbContext _context;

        public CreateEmployeeBankAccountCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CreateEmployeeBankAccountResponse> Handle(
            CreateEmployeeBankAccountCommand request,
            CancellationToken cancellationToken)
        {
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
            // DUPLICATE BANK ACCOUNT CHECK
            // =====================================================

            var duplicate = await _context.EmployeeBankAccounts
                .AnyAsync(
                    x =>
                        x.EmployeeId == dto.EmployeeId &&
                        x.AccountNumber == dto.AccountNumber,
                    cancellationToken);

            if (duplicate)
            {
                throw new EmployeeBankAccountValidationException(
                    "This bank account already exists for the employee.");
            }

            // =====================================================
            // PRIMARY ACCOUNT
            // =====================================================

            if (dto.IsPrimary == true)
            {
                var existingPrimaryAccounts =
                    await _context.EmployeeBankAccounts
                        .Where(x =>
                            x.EmployeeId == dto.EmployeeId &&
                            x.IsPrimary == true &&
                            x.IsActive == true)
                        .ToListAsync(cancellationToken);

                foreach (var account in existingPrimaryAccounts)
                {
                    account.IsPrimary = false;
                    account.UpdatedBy = request.CurrentUserId.ToString();
                    account.UpdatedDate = DateTime.UtcNow;
                }
            }

            // =====================================================
            // CREATE ENTITY
            // =====================================================

            var entity = new ERPWMS.Database.Models.EmployeeBankAccount
            {
                EmployeeId = dto.EmployeeId,

                BankName = dto.BankName.Trim(),

                AccountNumber = dto.AccountNumber.Trim(),

                Iban = string.IsNullOrWhiteSpace(dto.Iban)
                    ? null
                    : dto.Iban.Trim(),

                AccountType = string.IsNullOrWhiteSpace(dto.AccountType)
                    ? null
                    : dto.AccountType.Trim(),

                IsPrimary = dto.IsPrimary ?? false,

                IsActive = dto.IsActive ?? true,

                // ✅ FIX: int -> string
                CreatedBy = request.CurrentUserId.ToString(),

                CreatedDate = DateTime.UtcNow
            };

            _context.EmployeeBankAccounts.Add(entity);

            await _context.SaveChangesAsync(cancellationToken);

            // =====================================================
            // RESPONSE
            // =====================================================

            return new CreateEmployeeBankAccountResponse
            {
                BankAccountId = entity.BankAccountId,

                Message = "Employee bank account created successfully."
            };
        }
    }
}