using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Interfaces;

namespace ERPWMS.Infrastructure.Services
{
    public class OvertimeResult
    {
        public decimal OTHours { get; set; }
        public decimal OTRate { get; set; }
        public decimal OTMultiplier { get; set; }
        public decimal OTAmount { get; set; }
        public string Summary { get; set; } = string.Empty;
    }

    public interface IOvertimeCalculator
    {
        Task<OvertimeResult> CalculateAsync(int employeeId, decimal otHours, bool isHoliday, bool isWeeklyOff);
    }

    public class OvertimeCalculator : IOvertimeCalculator
    {
        private readonly AppDbContext _context;

        public OvertimeCalculator(AppDbContext context)
        {
            _context = context;
        }

        public async Task<OvertimeResult> CalculateAsync(
            int employeeId, decimal otHours,
            bool isHoliday, bool isWeeklyOff)
        {
            var result = new OvertimeResult { OTHours = otHours };

            // FIXED: Negative hours validation
            if (otHours < 0)
            {
                result.Summary = "OT hours cannot be negative.";
                return result;
            }

            if (otHours == 0)
            {
                result.Summary = "No overtime hours.";
                return result;
            }

            if (employeeId <= 0)
            {
                result.Summary = "Invalid employee ID.";
                return result;
            }

            var emp = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (emp == null)
            {
                result.Summary = "Employee not found.";
                return result;
            }

            var settings = await _context.PayrollSettings
                .OrderByDescending(p => p.SettingId)
                .FirstOrDefaultAsync();

            int divisor = settings?.SalaryDivisor ?? 30;
            decimal baseMultiplier = settings?.OTMultiplier ?? 1.5m;

            if (divisor <= 0) divisor = 30;

            result.OTRate = emp.BaseSalary / divisor / 8; // per hour

            // Holiday / Weekly Off pe double
            if (isHoliday || isWeeklyOff)
                result.OTMultiplier = baseMultiplier * 2;
            else
                result.OTMultiplier = baseMultiplier;

            result.OTAmount = Math.Round(
                result.OTRate * result.OTMultiplier * otHours, 2);

            result.Summary = $"{otHours} hrs x {result.OTRate:N2} x {result.OTMultiplier} = {result.OTAmount:N2}";
            return result;
        }
    }
}
