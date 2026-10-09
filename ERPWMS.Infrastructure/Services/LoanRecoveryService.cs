using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;
using ERPWMS.Database.Models;

namespace ERPWMS.Infrastructure.Services
{
    public interface ILoanRecoveryService
    {
        Task<bool> ApplyMonthlyRecoveryAsync(int employeeId, DateOnly period, int userId);
        Task<decimal> GetMonthlyRecoveryTotalAsync(int employeeId);
    }

    public class LoanRecoveryService : ILoanRecoveryService
    {
        private readonly AppDbContext _context;
        public LoanRecoveryService(AppDbContext context) { _context = context; }

        public async Task<decimal> GetMonthlyRecoveryTotalAsync(int employeeId)
        {
            var loans = await _context.Loans
                .Where(l => l.EmployeeId == employeeId && l.Status == "Active" && l.IsDeleted != true)
                .ToListAsync();
            return loans.Sum(l => l.MonthlyRecovery);
        }

        public async Task<bool> ApplyMonthlyRecoveryAsync(int employeeId, DateOnly period, int userId)
        {
            var loans = await _context.Loans
                .Where(l => l.EmployeeId == employeeId && l.Status == "Active" && l.IsDeleted != true)
                .ToListAsync();

            foreach (var loan in loans)
            {
                var recovery = new LoanRecovery
                {
                    LoanId = loan.LoanId,
                    RecoveryDate = period,
                    Amount = loan.MonthlyRecovery,
                    Status = "Paid",
                    IsProcessedInPayroll = true,
                    CreatedBy = userId.ToString(),
                    CreatedDate = DateTime.Now
                };
                _context.LoanRecoveries.Add(recovery);
                loan.TotalRecovered += loan.MonthlyRecovery;
                loan.RemainingAmount = loan.LoanAmount - loan.TotalRecovered;
                loan.PaidInstallments++;
                if (loan.RemainingAmount <= 0)
                {
                    loan.Status = "Closed";
                    loan.RemainingAmount = 0;
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
