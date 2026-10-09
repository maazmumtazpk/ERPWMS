using MediatR;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.EmployeeBankAccount;
using ERPWMS.Database;

using EmployeeNotFoundException =
    ERPWMS.Domain.Exceptions.HRMaster.EmployeeNotFoundException;

using EmployeeBankAccountNotFoundException =
    ERPWMS.Domain.Exceptions.EmployeeBankAccountNotFoundException;

namespace ERPWMS.Application.Queries.EmployeeBankAccount.GetEmployeeBankAccountDetails
{
    public class GetEmployeeBankAccountDetailsQueryHandler
        : IRequestHandler<
            GetEmployeeBankAccountDetailsQuery,
            EmployeeBankAccountDto>
    {
        private readonly AppDbContext _context;

        public GetEmployeeBankAccountDetailsQueryHandler(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<EmployeeBankAccountDto> Handle(
            GetEmployeeBankAccountDetailsQuery request,
            CancellationToken cancellationToken)
        {
            // CHECK EMPLOYEE
            var employeeExists = await _context.Employees
                .AnyAsync(
                    x => x.EmployeeId == request.EmployeeId,
                    cancellationToken);

            if (!employeeExists)
            {
                // ✅ FIX: int → string
                throw new EmployeeNotFoundException(
                    request.EmployeeId.ToString());
            }

            // GET BANK ACCOUNT
            var account = await _context.EmployeeBankAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x =>
                        x.BankAccountId == request.BankAccountId &&
                        x.EmployeeId == request.EmployeeId,
                    cancellationToken);

            if (account == null)
            {
                // ✅ FIX: int → string
                throw new EmployeeBankAccountNotFoundException(
                    request.BankAccountId.ToString());
            }

            // RETURN DTO
            return new EmployeeBankAccountDto
            {
                BankAccountId = account.BankAccountId,
                EmployeeId = account.EmployeeId,
                BankName = account.BankName,
                AccountNumber = account.AccountNumber,
                Iban = account.Iban,
                AccountType = account.AccountType,
                IsPrimary = account.IsPrimary,
                IsActive = account.IsActive,
                CreatedBy = account.CreatedBy,
                CreatedDate = account.CreatedDate,
                UpdatedBy = account.UpdatedBy,
                UpdatedDate = account.UpdatedDate
            };
        }
    }
}