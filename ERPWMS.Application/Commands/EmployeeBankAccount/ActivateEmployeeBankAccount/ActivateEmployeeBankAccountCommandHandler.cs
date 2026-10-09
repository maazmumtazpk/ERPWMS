using MediatR;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Exceptions;
using ERPWMS.Database;
using ERPWMS.Domain.Exceptions.HRMaster;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Commands.EmployeeBankAccount.ActivateEmployeeBankAccount
{
    public class ActivateEmployeeBankAccountCommandHandler
        : IRequestHandler<ActivateEmployeeBankAccountCommand, bool>
    {
        private readonly AppDbContext _context;

        public ActivateEmployeeBankAccountCommandHandler(
            AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(
            ActivateEmployeeBankAccountCommand request,
            CancellationToken cancellationToken)
        {
            // =====================================================
            // FIND BANK ACCOUNT
            // =====================================================

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

            // =====================================================
            // ACTIVATE ACCOUNT
            // =====================================================

            account.IsActive = true;
            account.UpdatedBy = request.CurrentUserId.ToString();
            account.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}