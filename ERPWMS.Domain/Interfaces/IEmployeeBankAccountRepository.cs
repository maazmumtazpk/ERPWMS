using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    public interface IEmployeeBankAccountRepository
    {
        // =========================================================
        // BASIC GET OPERATIONS
        // =========================================================

        Task<EmployeeBankAccount?> GetByIdAsync(int id);

        Task<EmployeeBankAccount?> GetByIdWithDetailsAsync(int id);

        Task<IEnumerable<EmployeeBankAccount>> GetByEmployeeIdAsync(
            int employeeId);

        Task<IEnumerable<EmployeeBankAccount>> GetActiveByEmployeeIdAsync(
            int employeeId);

        Task<EmployeeBankAccount?> GetPrimaryByEmployeeIdAsync(
            int employeeId);

        // =========================================================
        // PAGINATION
        // =========================================================

        Task<IEnumerable<EmployeeBankAccount>> GetPagedByEmployeeIdAsync(
            int employeeId,
            int page,
            int pageSize);

        Task<int> GetCountByEmployeeIdAsync(
            int employeeId);

        // =========================================================
        // VALIDATION
        // =========================================================

        Task<bool> AccountExistsAsync(
            string accountNumber,
            int? excludeId = null);

        Task<bool> AccountExistsForEmployeeAsync(
            int employeeId,
            string accountNumber,
            int? excludeId = null);

        Task<bool> EmployeeHasPrimaryAccountAsync(
            int employeeId,
            int? excludeId = null);

        // =========================================================
        // DASHBOARD
        // =========================================================

        Task<int> GetTotalCountAsync();

        // =========================================================
        // FILTER + SEARCH + PAGINATION
        // =========================================================

        Task<(List<EmployeeBankAccount> Items, int TotalCount)>
            SearchAsync(
                int? employeeId,
                string? searchTerm,
                string? bankName,
                bool? isActive,
                bool? isPrimary,
                DateTime? dateFrom,
                DateTime? dateTo,
                string? sortBy,
                bool sortDesc,
                int page,
                int pageSize);

        // =========================================================
        // REPORT
        // =========================================================

        Task<List<EmployeeBankAccount>> GetReportAsync(
            int? employeeId,
            string? searchTerm,
            string? bankName,
            bool? isActive,
            bool? isPrimary,
            DateTime? dateFrom,
            DateTime? dateTo);

        // =========================================================
        // CRUD
        // =========================================================

        Task AddAsync(EmployeeBankAccount account);

        void Update(EmployeeBankAccount account);

        Task SaveChangesAsync();
    }
}
