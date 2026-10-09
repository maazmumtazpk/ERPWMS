using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Employee operations in HR Master.
    /// </summary>
    public class EmployeeRepository(
        AppDbContext context,
        ILogger<EmployeeRepository> logger) : IEmployeeRepository
    {
        private IDbContextTransaction? _currentTransaction;

        // =========================================================
        // TRANSACTION MANAGEMENT
        // =========================================================

        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
                return;

            _currentTransaction = await context.Database.BeginTransactionAsync();

            logger.LogDebug("Employee transaction started.");
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction == null)
                return;

            try
            {
                await _currentTransaction.CommitAsync();
                logger.LogDebug("Employee transaction committed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while committing employee transaction.");
                throw;
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction == null)
                return;

            try
            {
                await _currentTransaction.RollbackAsync();
                logger.LogDebug("Employee transaction rolled back.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while rolling back employee transaction.");
                throw;
            }
            finally
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        // =========================================================
        // BASIC CRUD / GET OPERATIONS
        // =========================================================

        public async Task<Employee?> GetByIdAsync(int id)
        {
            return await context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .Include(e => e.Shift)
                .FirstOrDefaultAsync(e => e.EmployeeId == id && e.IsDeleted != true);
        }

        public async Task<Employee?> GetByCodeAsync(string employeeCode)
        {
            if (string.IsNullOrWhiteSpace(employeeCode))
                return null;

            employeeCode = employeeCode.Trim();

            return await context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.EmployeeCode == employeeCode &&
                    e.IsDeleted != true);
        }

        public async Task<Employee?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            logger.LogDebug(
                "GetByEmailAsync called, but Email is not a property of Employee.");

            return await Task.FromResult<Employee?>(null);
        }

        public async Task<Employee?> GetEmployeeWithDetailsAsync(int id)
        {
            return await context.Employees
                .AsNoTracking()
                .Where(e => e.EmployeeId == id && e.IsDeleted != true)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .Include(e => e.Warehouse)
                .Include(e => e.Shift)
                .Include(e => e.EmployeeBankAccounts)
                .Include(e => e.EmployeeBankAccounts)
                .Include(e => e.EmployeeSalaries)
                .Include(e => e.EmployeeSalaries)
                .Include(e => e.EmployeePayrolls)
                .Include(e => e.EmployeeLeaves)
                .FirstOrDefaultAsync();
        }

        public async Task<Employee?> GetDetailedAsync(int id)
        {
            return await context.Employees
                .AsNoTracking()
                .Where(e => e.EmployeeId == id && e.IsDeleted != true)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .Include(e => e.Warehouse)
                .Include(e => e.Shift)
                .Include(e => e.EmployeePayrolls)
                .Include(e => e.EmployeeLeaves)
                .Include(e => e.EmployeeSalaries)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Employee>> GetAllAsync()
        {
            return await context.Employees
                .AsNoTracking()
                .Where(e => e.IsDeleted != true)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetActiveEmployeesAsync()
        {
            return await context.Employees
                .AsNoTracking()
                .Where(e => e.IsActive == true && e.IsDeleted != true)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDepartmentAsync(int departmentId)
        {
            return await context.Employees
                .AsNoTracking()
                .Where(e => e.DepartmentId == departmentId && e.IsDeleted != true)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByDesignationAsync(int designationId)
        {
            return await context.Employees
                .AsNoTracking()
                .Where(e => e.DesignationId == designationId && e.IsDeleted != true)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByBranchAsync(int branchId)
        {
            return await context.Employees
                .AsNoTracking()
                .Where(e => e.BranchId == branchId && e.IsDeleted != true)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByShiftAsync(int shiftId)
        {
            return await context.Employees
                .AsNoTracking()
                .Where(e => e.ShiftId == shiftId && e.IsDeleted != true)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .Include(e => e.Shift)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> GetEmployeesByWarehouseAsync(int warehouseId)
        {
            return await context.Employees
                .AsNoTracking()
                .Where(e => e.WarehouseId == warehouseId && e.IsDeleted != true)
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .Include(e => e.Warehouse)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Employee>> SearchEmployeesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllAsync();

            var term = searchTerm.Trim();

            return await context.Employees
                .AsNoTracking()
                .Where(e =>
                    e.IsDeleted != true &&
                    (
                        (e.EmployeeCode != null && EF.Functions.Like(e.EmployeeCode, $"%{term}%"))
                        ||
                        (e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{term}%"))
                        ||
                        (e.LastName != null && EF.Functions.Like(e.LastName, $"%{term}%"))
                        ||
                        EF.Functions.Like(e.FirstName + " " + e.LastName, $"%{term}%")
                        ||
                        (e.Cnic != null && EF.Functions.Like(e.Cnic, $"%{term}%"))
                    ))
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToListAsync();
        }

        public async Task<(List<Employee> Items, int TotalCount)> SearchAsync(
            string? searchTerm,
            int? departmentId,
            int? designationId,
            int? branchId,
            int? warehouseId,
            bool? isActive,
            string? sortBy,
            bool sortDesc,
            int page,
            int pageSize)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var query = context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .Include(e => e.Shift)
                .Where(e => e.IsDeleted != true);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();

                query = query.Where(e =>
                    (e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{term}%"))
                    ||
                    (e.LastName != null && EF.Functions.Like(e.LastName, $"%{term}%"))
                    ||
                    (e.EmployeeCode != null && EF.Functions.Like(e.EmployeeCode, $"%{term}%"))
                    ||
                    (e.Cnic != null && EF.Functions.Like(e.Cnic, $"%{term}%"))
                    ||
                    EF.Functions.Like(e.FirstName + " " + e.LastName, $"%{term}%"));
            }

            if (departmentId.HasValue) query = query.Where(e => e.DepartmentId == departmentId);
            if (designationId.HasValue) query = query.Where(e => e.DesignationId == designationId);
            if (branchId.HasValue) query = query.Where(e => e.BranchId == branchId);
            if (warehouseId.HasValue) query = query.Where(e => e.WarehouseId == warehouseId);
            if (isActive.HasValue) query = query.Where(e => e.IsActive == isActive);

            var totalCount = await query.CountAsync();

            query = sortBy?.Trim().ToLowerInvariant() switch
            {
                "firstname" => sortDesc ? query.OrderByDescending(e => e.FirstName) : query.OrderBy(e => e.FirstName),
                "lastname" => sortDesc ? query.OrderByDescending(e => e.LastName) : query.OrderBy(e => e.LastName),
                "hiredate" => sortDesc ? query.OrderByDescending(e => e.HireDate) : query.OrderBy(e => e.HireDate),
                "basesalary" => sortDesc ? query.OrderByDescending(e => e.BaseSalary) : query.OrderBy(e => e.BaseSalary),
                "employeecode" => sortDesc ? query.OrderByDescending(e => e.EmployeeCode) : query.OrderBy(e => e.EmployeeCode),
                _ => query.OrderByDescending(e => e.EmployeeId)
            };

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IEnumerable<Employee>> GetPagedEmployeesAsync(
            int page, int pageSize, string? searchTerm, bool includeDeleted)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize < 1 ? 10 : pageSize;

            var query = context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Include(e => e.Branch)
                .Include(e => e.Shift)
                .AsQueryable();

            if (!includeDeleted) query = query.Where(e => e.IsDeleted != true);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();

                query = query.Where(e =>
                    (e.EmployeeCode != null && EF.Functions.Like(e.EmployeeCode, $"%{term}%"))
                    ||
                    (e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{term}%"))
                    ||
                    (e.LastName != null && EF.Functions.Like(e.LastName, $"%{term}%"))
                    ||
                    EF.Functions.Like(e.FirstName + " " + e.LastName, $"%{term}%")
                    ||
                    (e.Cnic != null && EF.Functions.Like(e.Cnic, $"%{term}%")));
            }

            return await query
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalCountAsync(string? searchTerm, bool includeDeleted)
        {
            var query = context.Employees
                .AsNoTracking()
                .AsQueryable();

            if (!includeDeleted) query = query.Where(e => e.IsDeleted != true);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();

                query = query.Where(e =>
                    (e.EmployeeCode != null && EF.Functions.Like(e.EmployeeCode, $"%{term}%"))
                    ||
                    (e.FirstName != null && EF.Functions.Like(e.FirstName, $"%{term}%"))
                    ||
                    (e.LastName != null && EF.Functions.Like(e.LastName, $"%{term}%"))
                    ||
                    EF.Functions.Like(e.FirstName + " " + e.LastName, $"%{term}%")
                    ||
                    (e.Cnic != null && EF.Functions.Like(e.Cnic, $"%{term}%")));
            }

            return await query.CountAsync();
        }

        public async Task<bool> EmployeeCodeExistsAsync(string employeeCode, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(employeeCode))
                return false;

            employeeCode = employeeCode.Trim();

            var query = context.Employees
                .Where(e => e.EmployeeCode == employeeCode && e.IsDeleted != true);

            if (excludeId.HasValue)
                query = query.Where(e => e.EmployeeId != excludeId);

            return await query.AnyAsync();
        }

        public async Task<bool> IsCodeUniqueAsync(string? code, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(code))
                return true;

            code = code.Trim();

            var query = context.Employees
                .Where(e => e.EmployeeCode == code && e.IsDeleted != true);

            if (excludeId.HasValue)
                query = query.Where(e => e.EmployeeId != excludeId);

            return !await query.AnyAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string? email, int? excludeId = null)
        {
            return await Task.FromResult(true);
        }

        public async Task<bool> EmployeeExistsAsync(int id)
        {
            return await context.Employees
                .AsNoTracking()
                .AnyAsync(e => e.EmployeeId == id && e.IsDeleted != true);
        }

        public async Task<bool> EmployeeHasActiveBankAccountsAsync(int employeeId)
        {
            return await context.EmployeeBankAccounts
                .AsNoTracking()
                .AnyAsync(ba => ba.EmployeeId == employeeId && ba.IsActive == true);
        }

        public async Task<bool> EmployeeHasBankAccountsAsync(int employeeId)
        {
            return await context.EmployeeBankAccounts
                .AsNoTracking()
                .AnyAsync(ba => ba.EmployeeId == employeeId);
        }

        public async Task<bool> EmployeeHasActiveSalaryAsync(int employeeId)
        {
            return await context.EmployeeSalaries
                .AsNoTracking()
                .AnyAsync(s => s.EmployeeId == employeeId && s.IsActive == true);
        }

        public async Task<bool> EmployeeHasSalariesAsync(int employeeId)
        {
            return await context.EmployeeSalaries
                .AsNoTracking()
                .AnyAsync(s => s.EmployeeId == employeeId);
        }

        public async Task<EmployeeAttendance?> GetAttendanceByDateAsync(int employeeId, DateOnly date)
        {
            return await context.EmployeeAttendances
                .AsNoTracking()
                .Include(a => a.Shift)
                .FirstOrDefaultAsync(a =>
                    a.EmployeeId == employeeId &&
                    a.AttendanceDate == date);
        }

        public async Task<List<EmployeeAttendance>> GetAttendanceByDateRangeAsync(
            int employeeId, DateOnly start, DateOnly end)
        {
            return await context.EmployeeAttendances
                .AsNoTracking()
                .Include(a => a.Shift)
                .Where(a => a.EmployeeId == employeeId &&
                            a.AttendanceDate >= start &&
                            a.AttendanceDate <= end)
                .OrderBy(a => a.AttendanceDate)
                .ToListAsync();
        }

        public async Task<List<EmployeeAttendance>> GetAttendanceReportAsync(
            int? departmentId,
            int? designationId,
            int? branchId,
            DateOnly? start,
            DateOnly? end,
            string? status)
        {
            var query = context.EmployeeAttendances
                .AsNoTracking()
                .Include(a => a.Shift)
                .AsQueryable();

            if (departmentId.HasValue)
                query = query.Where(a => context.Employees.Any(e =>
                    e.EmployeeId == a.EmployeeId &&
                    e.DepartmentId == departmentId &&
                    e.IsDeleted != true));

            if (designationId.HasValue)
                query = query.Where(a => context.Employees.Any(e =>
                    e.EmployeeId == a.EmployeeId &&
                    e.DesignationId == designationId &&
                    e.IsDeleted != true));

            if (branchId.HasValue)
                query = query.Where(a => context.Employees.Any(e =>
                    e.EmployeeId == a.EmployeeId &&
                    e.BranchId == branchId &&
                    e.IsDeleted != true));

            if (start.HasValue) query = query.Where(a => a.AttendanceDate >= start);
            if (end.HasValue) query = query.Where(a => a.AttendanceDate <= end);

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.Trim();
                query = query.Where(a => a.Status == status);
            }

            return await query.OrderBy(a => a.AttendanceDate).ToListAsync();
        }

        public async Task<EmployeePayroll?> GetPayrollByIdAsync(int payrollId)
        {
            return await context.EmployeePayrolls
                .AsNoTracking()
                .Include(p => p.Employee)
                .FirstOrDefaultAsync(p => p.PayrollId == payrollId);
        }

        public async Task<EmployeePayroll?> GetPayrollByPeriodAsync(int employeeId, DateOnly monthYear)
        {
            return await context.EmployeePayrolls
                .AsNoTracking()
                .FirstOrDefaultAsync(p =>
                    p.EmployeeId == employeeId &&
                    p.MonthYear == monthYear);
        }

        public async Task<List<EmployeePayroll>> GetPayrollByEmployeeAsync(int employeeId)
        {
            return await context.EmployeePayrolls
                .AsNoTracking()
                .Where(p => p.EmployeeId == employeeId)
                .OrderByDescending(p => p.MonthYear)
                .ToListAsync();
        }

        public async Task<List<EmployeePayroll>> GetPayrollReportAsync(
            int? departmentId,
            int? employeeId,
            DateOnly? periodStart,
            DateOnly? periodEnd,
            string? status)
        {
            var query = context.EmployeePayrolls
                .AsNoTracking()
                .Include(p => p.Employee)
                    .ThenInclude(e => e.Department)
                .Where(p => p.Employee != null);

            if (departmentId.HasValue) query = query.Where(p => p.Employee.DepartmentId == departmentId);
            if (employeeId.HasValue) query = query.Where(p => p.EmployeeId == employeeId);
            if (periodStart.HasValue) query = query.Where(p => p.MonthYear >= periodStart);
            if (periodEnd.HasValue) query = query.Where(p => p.MonthYear <= periodEnd);

            if (!string.IsNullOrWhiteSpace(status))
            {
                status = status.Trim();
                query = query.Where(p => p.Status == status);
            }

            return await query.OrderByDescending(p => p.MonthYear).ToListAsync();
        }

        public async Task<EmployeeSalary?> GetActiveSalaryStructureAsync(int employeeId)
        {
            return await context.EmployeeSalaries
                .AsNoTracking()
                .Where(s => s.EmployeeId == employeeId && s.IsActive == true)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefaultAsync();
        }

        public async Task<EmployeeSalary?> GetSalaryStructureByDateAsync(int employeeId, DateOnly date)
        {
            return await context.EmployeeSalaries
                .AsNoTracking()
                .Where(s => s.EmployeeId == employeeId &&
                            s.EffectiveDate <= date &&
                            s.IsActive == true)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefaultAsync();
        }

        public async Task<Shift?> GetShiftByIdAsync(int id)
        {
            return await context.Shifts
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ShiftId == id && s.IsDeleted != true);
        }

        public async Task<List<Shift>> GetAllShiftsAsync()
        {
            return await context.Shifts
                .AsNoTracking()
                .Where(s => s.IsActive == true && s.IsDeleted != true)
                .OrderBy(s => s.ShiftName)
                .ToListAsync();
        }

        public async Task<EmployeeLeave?> GetLeaveByIdAsync(int leaveId)
        {
            return await context.EmployeeLeaves
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.LeaveId == leaveId);
        }

        public async Task<List<EmployeeLeave>> GetLeavesByEmployeeAsync(int employeeId)
        {
            return await context.EmployeeLeaves
                .AsNoTracking()
                .Where(l => l.EmployeeId == employeeId)
                .OrderByDescending(l => l.CreatedDate)
                .ToListAsync();
        }

        // ? FIXED: int deletedBy ? string deletedBy (interface ke mutabiq)
        public async Task SoftDeleteAsync(int id, string deletedBy)
        {
            var employee = await context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id && e.IsDeleted != true);

            if (employee == null)
            {
                logger.LogWarning("Employee {EmployeeId} not found for soft delete.", id);
                return;
            }

            var now = DateTime.UtcNow;

            employee.IsDeleted = true;
            employee.IsActive = false;

            employee.DeletedBy = deletedBy;      // ? string = string
            employee.DeletedDate = now;

            employee.UpdatedBy = deletedBy;      // ? string = string
            employee.UpdatedDate = now;

            context.Employees.Update(employee);
        }

        public async Task AddAsync(Employee employee)
        {
            ArgumentNullException.ThrowIfNull(employee);
            await context.Employees.AddAsync(employee);
        }

        public void Update(Employee employee)
        {
            ArgumentNullException.ThrowIfNull(employee);
            context.Employees.Update(employee);
        }

        public Task UpdateAsync(Employee employee)
        {
            ArgumentNullException.ThrowIfNull(employee);
            context.Employees.Update(employee);
            return Task.CompletedTask;
        }

        public async Task AddAttendanceAsync(EmployeeAttendance attendance)
        {
            ArgumentNullException.ThrowIfNull(attendance);
            await context.EmployeeAttendances.AddAsync(attendance);
        }

        public Task UpdateAttendanceAsync(EmployeeAttendance attendance)
        {
            ArgumentNullException.ThrowIfNull(attendance);
            context.EmployeeAttendances.Update(attendance);
            return Task.CompletedTask;
        }

        public async Task AddPayrollAsync(EmployeePayroll payroll)
        {
            ArgumentNullException.ThrowIfNull(payroll);
            await context.EmployeePayrolls.AddAsync(payroll);
        }

        public Task UpdatePayrollAsync(EmployeePayroll payroll)
        {
            ArgumentNullException.ThrowIfNull(payroll);
            context.EmployeePayrolls.Update(payroll);
            return Task.CompletedTask;
        }

        public async Task AddLeaveAsync(EmployeeLeave leave)
        {
            ArgumentNullException.ThrowIfNull(leave);
            await context.EmployeeLeaves.AddAsync(leave);
        }

        public Task UpdateLeaveAsync(EmployeeLeave leave)
        {
            ArgumentNullException.ThrowIfNull(leave);
            context.EmployeeLeaves.Update(leave);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await context.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await context.SaveChangesAsync();
        }
    }
}