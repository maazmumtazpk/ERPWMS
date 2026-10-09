using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    public interface IEmployeeSalaryRepository
    {
        Task<EmployeeSalary?> GetByIdAsync(int id);
        Task<EmployeeSalary?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<EmployeeSalary>> GetByEmployeeIdAsync(int employeeId);
        Task<IEnumerable<EmployeeSalary>> GetActiveByEmployeeIdAsync(int employeeId);
        Task<EmployeeSalary?> GetLatestByEmployeeIdAsync(int employeeId);
        Task<EmployeeSalary?> GetActiveByEmployeeIdAndDateAsync(int employeeId, DateOnly date);
        Task<IEnumerable<EmployeeSalary>> GetPagedByEmployeeIdAsync(int employeeId, int page, int pageSize);
        Task<int> GetCountByEmployeeIdAsync(int employeeId);
        Task<bool> SalaryExistsForEmployeeAndDateAsync(int employeeId, DateOnly effectiveDate, int? excludeId = null);
        Task<EmployeeSalary?> GetActiveSalaryByEmployeeIdAsync(int employeeId);

        // 🆕 NEW: Total count for all salary records (for dashboard)
        Task<int> GetTotalCountAsync();

        Task AddAsync(EmployeeSalary salary);
        void Update(EmployeeSalary salary);
        Task SaveChangesAsync();
    }
}
