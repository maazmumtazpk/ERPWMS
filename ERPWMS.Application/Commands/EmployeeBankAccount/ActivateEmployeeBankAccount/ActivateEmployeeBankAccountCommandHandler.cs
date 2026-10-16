using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Exceptions;

namespace ERPWMS.Application.Commands.EmployeeBankAccount.ActivateEmployeeBankAccount
{
    public class ActivateEmployeeBankAccountCommandHandler
        : IRequestHandler<ActivateEmployeeBankAccountCommand, bool>
    {
        private readonly AppDbContext _context;

        public ActivateEmployeeBankAccountCommandHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            ActivateEmployeeBankAccountCommand request,
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

            // Already active
            if (account.IsActive == true)
            {
                return true;
            }

            // Activate
            account.IsActive = true;
            account.UpdatedBy = request.CurrentUserId.ToString();
            account.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
