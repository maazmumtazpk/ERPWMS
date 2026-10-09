using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Employee operations in HR Master
    /// </summary>
    public interface IEmployeeRepository
    {
        // =========================================================
        // TRANSACTION MANAGEMENT
        // =========================================================

        /// <summary>
        /// Begin a database transaction
        /// </summary>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commit the current transaction
        /// </summary>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rollback the current transaction
        /// </summary>
        Task RollbackTransactionAsync();

        // =========================================================
        // BASIC CRUD OPERATIONS
        // =========================================================

        /// <summary>
        /// Get employee by ID
        /// </summary>
        Task<Employee?> GetByIdAsync(int id);

        /// <summary>
        /// Get employee by code
        /// </summary>
        Task<Employee?> GetByCodeAsync(string employeeCode);

        /// <summary>
        /// Get employee by email
        /// </summary>
        Task<Employee?> GetByEmailAsync(string email);

        /// <summary>
        /// Get employee with all details (includes navigation properties)
        /// </summary>
        Task<Employee?> GetEmployeeWithDetailsAsync(int id);

        /// <summary>
        /// Get employee with detailed information (includes Payrolls, Leaves, Salaries)
        /// </summary>
        Task<Employee?> GetDetailedAsync(int id);

        /// <summary>
        /// Get all employees
        /// </summary>
        Task<IEnumerable<Employee>> GetAllAsync();

        /// <summary>
        /// Get all active employees
        /// </summary>
        Task<IEnumerable<Employee>> GetActiveEmployeesAsync();

        // =========================================================
        // FILTERING OPERATIONS
        // =========================================================

        /// <summary>
        /// Get employees by department
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId);

        /// <summary>
        /// Get employees by designation
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByDesignationAsync(int designationId);

        /// <summary>
        /// Get employees by branch
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByBranchAsync(int branchId);

        /// <summary>
        /// Get employees by shift
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByShiftAsync(int shiftId);

        /// <summary>
        /// Get employees by warehouse
        /// </summary>
        Task<IEnumerable<Employee>> GetEmployeesByWarehouseAsync(int warehouseId);

        // =========================================================
        // SEARCH OPERATIONS
        // =========================================================

        /// <summary>
        /// Search employees by search term
        /// </summary>
        Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm);

        /// <summary>
        /// Advanced search with multiple filters, sorting and pagination
        /// </summary>
        Task<(List<Employee> Items, int TotalCount)> SearchAsync(
            string? searchTerm,
            int? departmentId,
            int? designationId,
            int? branchId,
            int? warehouseId,
            bool? isActive,
            string? sortBy,
            bool sortDesc,
            int page,
            int pageSize);

        /// <summary>
        /// Get paged employees with search
        /// </summary>
        Task<IEnumerable<Employee>> GetPagedEmployeesAsync(
            int page,
            int pageSize,
            string? searchTerm,
            bool includeDeleted);

        /// <summary>
        /// Get total count of employees
        /// </summary>
        Task<int> GetTotalCountAsync(string? searchTerm, bool includeDeleted);

        // =========================================================
        // VALIDATION OPERATIONS
        // =========================================================

        /// <summary>
        /// Check if employee code exists
        /// </summary>
        Task<bool> EmployeeCodeExistsAsync(string employeeCode, int? excludeId = null);

        /// <summary>
        /// Check if employee code is unique
        /// </summary>
        Task<bool> IsCodeUniqueAsync(string? code, int? excludeId = null);

        /// <summary>
        /// Check if employee email is unique
        /// </summary>
        Task<bool> IsEmailUniqueAsync(string? email, int? excludeId = null);

        /// <summary>
        /// Check if employee exists
        /// </summary>
        Task<bool> EmployeeExistsAsync(int id);

        /// <summary>
        /// Check if employee has active bank accounts
        /// </summary>
        Task<bool> EmployeeHasActiveBankAccountsAsync(int employeeId);

        /// <summary>
        /// Check if employee has active salary
        /// </summary>
        Task<bool> EmployeeHasActiveSalaryAsync(int employeeId);

        /// <summary>
        /// Check if employee has any bank account
        /// </summary>
        Task<bool> EmployeeHasBankAccountsAsync(int employeeId);

        /// <summary>
        /// Check if employee has any salary record
        /// </summary>
        Task<bool> EmployeeHasSalariesAsync(int employeeId);

        // =========================================================
        // ATTENDANCE OPERATIONS
        // =========================================================

        /// <summary>
        /// Get attendance by date for an employee
        /// </summary>
        Task<EmployeeAttendance?> GetAttendanceByDateAsync(int employeeId, DateOnly date);

        /// <summary>
        /// Get attendance by date range for an employee
        /// </summary>
        Task<List<EmployeeAttendance>> GetAttendanceByDateRangeAsync(
            int employeeId,
            DateOnly start,
            DateOnly end);

        /// <summary>
        /// Get attendance report with filters
        /// </summary>
        Task<List<EmployeeAttendance>> GetAttendanceReportAsync(
            int? departmentId,
            int? designationId,
            int? branchId,
            DateOnly? start,
            DateOnly? end,
            string? status);

        // =========================================================
        // PAYROLL OPERATIONS
        // =========================================================

        /// <summary>
        /// Get payroll by ID
        /// </summary>
        Task<EmployeePayroll?> GetPayrollByIdAsync(int payrollId);

        /// <summary>
        /// Get payroll by period for an employee
        /// </summary>
        Task<EmployeePayroll?> GetPayrollByPeriodAsync(int employeeId, DateOnly monthYear);

        /// <summary>
        /// Get all payrolls for an employee
        /// </summary>
        Task<List<EmployeePayroll>> GetPayrollByEmployeeAsync(int employeeId);

        /// <summary>
        /// Get payroll report with filters
        /// </summary>
        Task<List<EmployeePayroll>> GetPayrollReportAsync(
            int? departmentId,
            int? employeeId,
            DateOnly? periodStart,
            DateOnly? periodEnd,
            string? status);

        // =========================================================
        // SALARY OPERATIONS
        // =========================================================

        /// <summary>
        /// Get active salary structure for an employee
        /// </summary>
        Task<EmployeeSalary?> GetActiveSalaryStructureAsync(int employeeId);

        /// <summary>
        /// Get salary structure by date for an employee
        /// </summary>
        Task<EmployeeSalary?> GetSalaryStructureByDateAsync(int employeeId, DateOnly date);

        // =========================================================
        // SHIFT OPERATIONS
        // =========================================================

        /// <summary>
        /// Get shift by ID
        /// </summary>
        Task<Shift?> GetShiftByIdAsync(int id);

        /// <summary>
        /// Get all active shifts
        /// </summary>
        Task<List<Shift>> GetAllShiftsAsync();

        // =========================================================
        // LEAVE OPERATIONS
        // =========================================================

        /// <summary>
        /// Get leave by ID
        /// </summary>
        Task<EmployeeLeave?> GetLeaveByIdAsync(int leaveId);

        /// <summary>
        /// Get all leaves for an employee
        /// </summary>
        Task<List<EmployeeLeave>> GetLeavesByEmployeeAsync(int employeeId);

        // =========================================================
        // COMMAND OPERATIONS
        // =========================================================

        /// <summary>
        /// Add new employee
        /// </summary>
        Task AddAsync(Employee employee);

        /// <summary>
        /// Update existing employee
        /// </summary>
        void Update(Employee employee);

        /// <summary>
        /// Update existing employee (async version)
        /// </summary>
        Task UpdateAsync(Employee employee);

        /// <summary>
        /// Add attendance record
        /// </summary>
        Task AddAttendanceAsync(EmployeeAttendance attendance);

        /// <summary>
        /// Add payroll record
        /// </summary>
        Task AddPayrollAsync(EmployeePayroll payroll);

        /// <summary>
        /// Add leave record
        /// </summary>
        Task AddLeaveAsync(EmployeeLeave leave);

        /// <summary>
        /// Update attendance record
        /// </summary>
        Task UpdateAttendanceAsync(EmployeeAttendance attendance);

        /// <summary>
        /// Update payroll record
        /// </summary>
        Task UpdatePayrollAsync(EmployeePayroll payroll);

        /// <summary>
        /// Update leave record
        /// </summary>
        Task UpdateLeaveAsync(EmployeeLeave leave);

        /// <summary>
        /// Soft delete employee
        /// </summary>
        Task SoftDeleteAsync(int id, string deletedBy);

        /// <summary>
        /// Save changes to database
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Save changes to database (alias)
        /// </summary>
        Task SaveAsync();
    }
}



