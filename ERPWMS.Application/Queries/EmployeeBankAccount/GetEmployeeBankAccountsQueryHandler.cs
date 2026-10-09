using MediatR;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Application.DTOs.EmployeeBankAccount;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Database;

namespace ERPWMS.Application.Queries.EmployeeBankAccount.GetEmployeeBankAccounts
{
    public class GetEmployeeBankAccountsQueryHandler
        : IRequestHandler<
            GetEmployeeBankAccountsQuery,
            IEnumerable<EmployeeBankAccountDto>>
    {
        private readonly AppDbContext _context;

        public GetEmployeeBankAccountsQueryHandler(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeBankAccountDto>> Handle(
            GetEmployeeBankAccountsQuery request,
            CancellationToken cancellationToken)
        {
            var employeeExists = await _context.Employees
                .AnyAsync(
                    x => x.EmployeeId == request.EmployeeId,
                    cancellationToken);

            if (!employeeExists)
            {
                throw new EmployeeNotFoundException(
                    request.EmployeeId);
            }

            return await _context.EmployeeBankAccounts
                .AsNoTracking()
                .Where(x => x.EmployeeId == request.EmployeeId)
                .OrderByDescending(x => x.IsPrimary)
                .ThenBy(x => x.BankName)
                .Select(x => new EmployeeBankAccountDto
                {
                    BankAccountId = x.BankAccountId,
                    EmployeeId = x.EmployeeId,
                    BankName = x.BankName,
                    AccountNumber = x.AccountNumber,
                    Iban = x.Iban,
                    AccountType = x.AccountType,
                    IsPrimary = x.IsPrimary,
                    IsActive = x.IsActive,
                    CreatedBy = x.CreatedBy,
                    CreatedDate = x.CreatedDate,
                    UpdatedBy = x.UpdatedBy,
                    UpdatedDate = x.UpdatedDate
                })
                .ToListAsync(cancellationToken);
        }
    }
}