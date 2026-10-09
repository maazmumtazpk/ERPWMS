using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Interfaces;

namespace ERPWMS.Infrastructure.Services
{
    public class AbsenceResult
    {
        public int EmployeeId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int AbsentDays { get; set; }
        public decimal DeductionDays { get; set; }
        public decimal DailyRate { get; set; }
        public decimal TotalDeduction { get; set; }
        public string Summary { get; set; } = string.Empty;
    }

    public interface IAbsenceCalculator
    {
        Task<AbsenceResult> CalculateAsync(int employeeId, int year, int month);
    }

    public class AbsenceCalculator : IAbsenceCalculator
    {
        private readonly AppDbContext _context;

        public AbsenceCalculator(AppDbContext context)
        {
            _context = context;
        }

        public async Task<AbsenceResult> CalculateAsync(int employeeId, int year, int month)
        {
            var result = new AbsenceResult { EmployeeId = employeeId, Year = year, Month = month };

            var emp = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
            if (emp == null) { result.Summary = "Employee not found."; return result; }

            var settings = await _context.PayrollSettings.OrderByDescending(p => p.SettingId).FirstOrDefaultAsync();
            int salaryDivisor = settings?.SalaryDivisor ?? 30;

            result.DailyRate = salaryDivisor > 0 ? emp.BaseSalary / salaryDivisor : 0;

            var monthStart = new DateOnly(year, month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            var absences = await _context.AbsenceRecords
                .Where(a => a.EmployeeId == employeeId
                    && a.AbsenceDate >= monthStart
                    && a.AbsenceDate <= monthEnd
                    && a.IsDeleted != true)
                .ToListAsync();

            result.AbsentDays = absences.Count;
            result.DeductionDays = absences.Sum(a => a.DeductionDays);
            result.TotalDeduction = result.DeductionDays * result.DailyRate;

            result.Summary = $"Absent: {result.AbsentDays} days | Deduction: {result.DeductionDays} days | Amount: {result.TotalDeduction:N2}";

            return result;
        }
    }
}
