using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Interfaces;

namespace ERPWMS.Infrastructure.Services
{
    public interface ILeaveBalanceService
    {
        Task<LeaveBalance?> GetBalanceAsync(int employeeId, int leaveTypeId, int year);
        Task AllocateAsync(int employeeId, int leaveTypeId, int year, decimal days, int currentUserId);
        Task<bool> DeductAsync(int employeeId, int leaveTypeId, decimal days, int currentUserId);
        Task<bool> ReleaseAsync(int employeeId, int leaveTypeId, decimal days);
        Task<bool> ConfirmUsageAsync(int employeeId, int leaveTypeId, decimal days);
    }

    public class LeaveBalanceService : ILeaveBalanceService
    {
        private readonly AppDbContext _context;

        public LeaveBalanceService(AppDbContext context) { _context = context; }

        public async Task<LeaveBalance?> GetBalanceAsync(int employeeId, int leaveTypeId, int year)
        {
            return await _context.LeaveBalances
                .FirstOrDefaultAsync(b => b.EmployeeId == employeeId
                    && b.LeaveTypeId == leaveTypeId
                    && b.Year == year
                    && b.IsDeleted != true);
        }

        public async Task AllocateAsync(int employeeId, int leaveTypeId, int year, decimal days, int currentUserId)
        {
            var bal = await GetBalanceAsync(employeeId, leaveTypeId, year);
            if (bal == null)
            {
                bal = new LeaveBalance
                {
                    EmployeeId = employeeId,
                    LeaveTypeId = leaveTypeId,
                    Year = year,
                    Allocated = days,
                    Remaining = days,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = currentUserId.ToString(),
                    CreatedDate = DateTime.Now
                };
                _context.LeaveBalances.Add(bal);
            }
            else
            {
                bal.Allocated += days;
                bal.Remaining = bal.Allocated + bal.CarriedForward - bal.Used - bal.Pending;
                bal.UpdatedBy = currentUserId.ToString();
                bal.UpdatedDate = DateTime.Now;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeductAsync(int employeeId, int leaveTypeId, decimal days, int currentUserId)
        {
            var year = DateTime.Now.Year;
            var bal = await GetBalanceAsync(employeeId, leaveTypeId, year);
            if (bal == null) return false;
            if (bal.Remaining < days) return false;

            bal.Pending += days;
            bal.Remaining = bal.Allocated + bal.CarriedForward - bal.Used - bal.Pending;
            bal.UpdatedBy = currentUserId.ToString();
            bal.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReleaseAsync(int employeeId, int leaveTypeId, decimal days)
        {
            var year = DateTime.Now.Year;
            var bal = await GetBalanceAsync(employeeId, leaveTypeId, year);
            if (bal == null) return false;

            bal.Pending = Math.Max(0, bal.Pending - days);
            bal.Remaining = bal.Allocated + bal.CarriedForward - bal.Used - bal.Pending;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ConfirmUsageAsync(int employeeId, int leaveTypeId, decimal days)
        {
            var year = DateTime.Now.Year;
            var bal = await GetBalanceAsync(employeeId, leaveTypeId, year);
            if (bal == null) return false;

            bal.Pending = Math.Max(0, bal.Pending - days);
            bal.Used += days;
            bal.Remaining = bal.Allocated + bal.CarriedForward - bal.Used - bal.Pending;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
