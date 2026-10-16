using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Exceptions;

namespace ERPWMS.Application.Commands.EmployeeBankAccount.DeactivateEmployeeBankAccount
{
    public class DeactivateEmployeeBankAccountCommandHandler
        : IRequestHandler<DeactivateEmployeeBankAccountCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeactivateEmployeeBankAccountCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            DeactivateEmployeeBankAccountCommand request,
            CancellationToken cancellationToken)
        {
            // Find bank account
            var account = await _context.EmployeeBankAccounts
                .FirstOrDefaultAsync(
                    x => x.BankAccountId == request.BankAccountId
                        && x.EmployeeId == request.EmployeeId,
                    cancellationToken);

            if (account == null)
            {
                throw new EmployeeBankAccountNotFoundException(
                    request.BankAccountId.ToString());
            }

            // Primary account cannot be deactivated
            if (account.IsPrimary == true)
            {
                throw new EmployeeBankAccountPrimaryException(
                    account.BankAccountId.ToString());
            }

            // Already inactive
            if (account.IsActive != true)
            {
                return true;
            }

            // Deactivate
            account.IsActive = false;
            account.UpdatedBy = request.CurrentUserId.ToString();
            account.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
