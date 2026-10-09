using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Interfaces;

namespace ERPWMS.Infrastructure.Services
{
    public class LatePolicyEngine : ILatePolicyEngine
    {
        private readonly AppDbContext _context;

        public LatePolicyEngine(AppDbContext context)
        {
            _context = context;
        }

        public async Task<LatePolicyResult> EvaluateAsync(int employeeId, int year, int month)
        {
            var result = new LatePolicyResult
            {
                EmployeeId = employeeId,
                Year = year,
                Month = month
            };

            var settings = await _context.PayrollSettings
                .OrderByDescending(p => p.SettingId)
                .FirstOrDefaultAsync();

            if (settings == null)
            {
                result.Summary = "No payroll settings found.";
                return result;
            }

            var monthStart = new DateOnly(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var attendances = await _context.EmployeeAttendances
                .Where(a => a.EmployeeId == employeeId
                    && a.AttendanceDate >= monthStart
                    && a.AttendanceDate <= monthEnd)
                .ToListAsync();

            var lateDays = attendances.Where(a => (a.LateMinutes ?? 0) > 0).ToList();
            var earlyDays = attendances.Where(a => (a.EarlyMinutes ?? 0) > 0).ToList();

            result.TotalLateDays = lateDays.Count;
            result.TotalLateMinutes = lateDays.Sum(a => a.LateMinutes ?? 0);
            result.TotalEarlyDays = earlyDays.Count;
            result.TotalEarlyMinutes = earlyDays.Sum(a => a.EarlyMinutes ?? 0);

            // Calculate half/full day deductions
            if (settings.EnableLateDeduction)
            {
                result.LateFullDays = result.TotalLateDays / Math.Max(1, settings.LateFullDayCount);
                result.LateHalfDays = (result.TotalLateDays % Math.Max(1, settings.LateFullDayCount)) / Math.Max(1, settings.LateHalfDayCount);
            }

            if (settings.EnableEarlyDeduction)
            {
                result.EarlyHalfDays = result.TotalEarlyDays / Math.Max(1, settings.EarlyHalfDayCount);
            }

            result.TotalDeductionDays = result.LateFullDays + (result.LateHalfDays * 0.5m) + (result.EarlyHalfDays * 0.5m);

            // Warning level
            if (result.TotalLateDays >= settings.LateWarningCount2)
                result.WarningLevel = "Final Warning";
            else if (result.TotalLateDays >= settings.LateWarningCount1)
                result.WarningLevel = "Warning";
            else
                result.WarningLevel = "None";

            result.Summary = $"Late: {result.TotalLateDays} days, Early: {result.TotalEarlyDays} days. " +
                             $"Deduction: {result.TotalDeductionDays} days. Warning: {result.WarningLevel}";

            return result;
        }
    }
}

