using MediatR;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions;
using ERPWMS.Database;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Commands.EmployeeBankAccount.DeactivateEmployeeBankAccount
{
    public class DeactivateEmployeeBankAccountCommandHandler
        : IRequestHandler<DeactivateEmployeeBankAccountCommand, bool>
    {
        private readonly AppDbContext _context;

        public DeactivateEmployeeBankAccountCommandHandler(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            DeactivateEmployeeBankAccountCommand request,
            CancellationToken cancellationToken)
        {
            // FIND BANK ACCOUNT
            var account = await _context.EmployeeBankAccounts
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

            // PRIMARY ACCOUNT CHECK
            if (account.IsPrimary == true)
            {
                // ✅ FIX: int → string
                throw new EmployeeBankAccountPrimaryException(
                    account.BankAccountId.ToString());
            }

            // DEACTIVATE
            account.IsActive = false;
            account.UpdatedBy = request.CurrentUserId.ToString();
            account.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}