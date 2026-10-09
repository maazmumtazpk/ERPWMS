using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Services;

// ✅ Sirf HRMaster namespace use karo — root wala chhod do
using ERPWMS.Domain.Exceptions.HRMaster;

// ✅ Root wali exceptions ko alias karo (jo HRMaster mein nahi hain)
using RootPayrollExistsEx = ERPWMS.Domain.Exceptions.EmployeePayrollAlreadyExistsException;
using RootPayrollNotFoundEx = ERPWMS.Domain.Exceptions.EmployeePayrollNotFoundException;
using RootSalaryStructureEx = ERPWMS.Domain.Exceptions.EmployeeSalaryStructureNotFoundException;

namespace ERPWMS.Infrastructure.Services
{
    public class EmployeeDomainService : IEmployeeDomainService
    {
        private readonly AppDbContext _context;

        public EmployeeDomainService(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // 1. ENSURE UNIQUE CODE
        // =========================================================
        public async Task EnsureUniqueCodeAsync(string code, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new EmployeeValidationException("Employee code is required.");

            var query = _context.Employees.Where(e =>
                e.EmployeeCode == code && e.IsDeleted != true);

            if (excludeId.HasValue)
                query = query.Where(e => e.EmployeeId != excludeId.Value);

            var exists = await query.AnyAsync();

            if (exists)
                throw new EmployeeCodeAlreadyExistsException(code);
        }

        // =========================================================
        // 2. ENSURE UNIQUE CNIC
        // =========================================================
        public async Task EnsureUniqueCnicAsync(string? cnic, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(cnic))
                return;

            var query = _context.Employees.Where(e =>
                e.Cnic == cnic && e.IsDeleted != true);

            if (excludeId.HasValue)
                query = query.Where(e => e.EmployeeId != excludeId.Value);

            var exists = await query.AnyAsync();

            if (exists)
                throw new EmployeeValidationException(
                    $"Employee with CNIC '{cnic}' already exists.");
        }

        // =========================================================
        // 3. REGISTER EMPLOYEE
        // =========================================================
        public async Task<Employee> RegisterEmployeeAsync(Employee employee)
        {
            if (employee == null)
                throw new ArgumentNullException(nameof(employee));

            // Validate code uniqueness
            await EnsureUniqueCodeAsync(employee.EmployeeCode);

            // Validate CNIC uniqueness
            await EnsureUniqueCnicAsync(employee.Cnic);

            // Validate branch
            if (employee.BranchId <= 0)
                throw new EmployeeValidationException("Branch is required.");

            var branchExists = await _context.Branches
                .AnyAsync(b => b.BranchId == employee.BranchId && b.IsDeleted != true);

            if (!branchExists)
                throw new EmployeeValidationException("Selected branch does not exist.");

            // Validate department
            if (employee.DepartmentId <= 0)
                throw new EmployeeValidationException("Department is required.");

            var deptExists = await _context.Departments
                .AnyAsync(d => d.DepartmentId == employee.DepartmentId && d.IsDeleted != true);

            if (!deptExists)
                throw new EmployeeValidationException("Selected department does not exist.");

            // Validate designation
            if (employee.DesignationId <= 0)
                throw new EmployeeValidationException("Designation is required.");

            var desigExists = await _context.Designations
                .AnyAsync(d => d.DesignationId == employee.DesignationId && d.IsDeleted != true);

            if (!desigExists)
                throw new EmployeeValidationException("Selected designation does not exist.");

            // Set defaults
            employee.IsActive = true;
            employee.IsDeleted = false;
            employee.CreatedDate = DateTime.UtcNow;

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return employee;
        }

        // =========================================================
        // 4. DEACTIVATE EMPLOYEE
        // =========================================================
        public async Task DeactivateEmployeeAsync(
            int employeeId,
            string? reason,
            DateOnly? terminationDate,
            string updatedByUserId)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.IsDeleted != true);

            if (employee == null)
                throw new EmployeeNotFoundException(employeeId);

            if (employee.IsActive != true)
                throw new EmployeeValidationException(
                    $"Employee {employee.EmployeeCode} is already inactive.");

            employee.IsActive = false;
            employee.UpdatedBy = updatedByUserId;
            employee.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // 5. ACTIVATE EMPLOYEE
        // =========================================================
        public async Task ActivateEmployeeAsync(int employeeId, string updatedByUserId)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.IsDeleted != true);

            if (employee == null)
                throw new EmployeeNotFoundException(employeeId);

            if (employee.IsActive == true)
                throw new EmployeeValidationException(
                    $"Employee {employee.EmployeeCode} is already active.");

            employee.IsActive = true;
            employee.UpdatedBy = updatedByUserId;
            employee.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // 6. ASSIGN SALARY STRUCTURE
        // =========================================================
        public async Task<EmployeeSalary> AssignSalaryStructureAsync(
            int employeeId,
            decimal basicSalary,
            decimal? houseRent,
            decimal? utilityAllowance,
            decimal? medicalAllowance,
            decimal? otherAllowances,
            decimal? incomeTax,
            decimal? professionalTax,
            decimal? otherDeductions,
            DateOnly effectiveDate,
            string createdByUserId)
        {
            // Validate employee
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.IsDeleted != true);

            if (employee == null)
                throw new EmployeeNotFoundException(employeeId);

            if (employee.IsActive != true)
                throw new EmployeeInactiveException(employeeId);

            // Validate basic salary
            if (basicSalary < 0)
                throw new EmployeeSalaryDomainException(
                    "Basic salary cannot be negative.", "ESL-000");

            // Calculate totals
            var totalAllowances =
                (houseRent ?? 0) +
                (utilityAllowance ?? 0) +
                (medicalAllowance ?? 0) +
                (otherAllowances ?? 0);

            var totalDeductions =
                (incomeTax ?? 0) +
                (professionalTax ?? 0) +
                (otherDeductions ?? 0);

            var netSalary = basicSalary + totalAllowances - totalDeductions;

            // Deactivate old salary if exists
            var existingSalaries = await _context.EmployeeSalaries
                .Where(s => s.EmployeeId == employeeId && s.IsActive == true)
                .ToListAsync();

            foreach (var oldSalary in existingSalaries)
            {
                oldSalary.IsActive = false;
                oldSalary.UpdatedBy = createdByUserId;
                oldSalary.UpdatedDate = DateTime.UtcNow;
            }

            // Create new salary
            var salary = new EmployeeSalary
            {
                EmployeeId = employeeId,
                BasicSalary = basicSalary,
                HouseRent = houseRent ?? 0,
                UtilityAllowance = utilityAllowance ?? 0,
                MedicalAllowance = medicalAllowance ?? 0,
                OtherAllowances = otherAllowances ?? 0,
                IncomeTax = incomeTax ?? 0,
                ProfessionalTax = professionalTax ?? 0,
                OtherDeductions = otherDeductions ?? 0,
                TotalAllowances = totalAllowances,
                TotalDeductions = totalDeductions,
                NetSalary = netSalary,
                IsActive = true,
                CreatedBy = createdByUserId,
                CreatedDate = DateTime.UtcNow
            };

            _context.EmployeeSalaries.Add(salary);
            await _context.SaveChangesAsync();

            return salary;
        }

        // =========================================================
        // 7. GENERATE PAYROLL
        // =========================================================
        public async Task<EmployeePayroll> GeneratePayrollAsync(
            int employeeId,
            DateOnly monthYear,
            string createdByUserId)
        {
            // Validate employee
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId && e.IsDeleted != true);

            if (employee == null)
                throw new EmployeeNotFoundException(employeeId);

            if (employee.IsActive != true)
                throw new EmployeeInactiveException(employeeId);

            // Get active salary structure
            var salary = await _context.EmployeeSalaries
                .Where(s => s.EmployeeId == employeeId && s.IsActive == true)
                .OrderByDescending(s => s.CreatedDate)
                .FirstOrDefaultAsync();

            if (salary == null)
                throw new RootSalaryStructureEx(employeeId);

            // Check duplicate payroll for the month
            var alreadyExists = await _context.EmployeePayrolls
                .AnyAsync(p =>
                    p.EmployeeId == employeeId &&
                    p.SalaryId == salary.SalaryId &&
                    p.MonthYear.Year == monthYear.Year &&
                    p.MonthYear.Month == monthYear.Month);

            if (alreadyExists)
                throw new RootPayrollExistsEx(employeeId, monthYear);

            // Create payroll
            var grossPay = salary.BasicSalary + (salary.TotalAllowances ?? 0);
            var totalDeductions = salary.TotalDeductions ?? 0;
            var netPay = grossPay - totalDeductions;

            var payroll = new EmployeePayroll
            {
                EmployeeId = employeeId,
                SalaryId = salary.SalaryId,
                MonthYear = monthYear,           // ✅ DateOnly
                GrossPay = grossPay,
                TotalDeductions = totalDeductions,
                NetPay = netPay,
                Status = "Draft",
                CreatedBy = createdByUserId,
                CreatedDate = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.EmployeePayrolls.Add(payroll);
            await _context.SaveChangesAsync();

            return payroll;
        }

        // =========================================================
        // 8. APPROVE PAYROLL
        // =========================================================
        public async Task ApprovePayrollAsync(int payrollId, int approvedByUserId)
        {
            var payroll = await _context.EmployeePayrolls
                .FirstOrDefaultAsync(p => p.PayrollId == payrollId);

            if (payroll == null)
                throw new RootPayrollNotFoundEx(payrollId);

            if (payroll.Status == "Approved")
                throw new EmployeeValidationException("Payroll is already approved.");

            if (payroll.Status == "Paid")
                throw new EmployeeValidationException("Payroll is already paid.");

            payroll.Status = "Approved";
            payroll.ApprovedBy = approvedByUserId.ToString();
            payroll.ApprovedDate = DateTime.UtcNow;
            payroll.UpdatedBy = approvedByUserId.ToString();
            payroll.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }

        // =========================================================
        // 9. MARK PAYROLL AS PAID
        // =========================================================
        public async Task MarkPayrollPaidAsync(
            int payrollId,
            DateTime paymentDate,
            string updatedByUserId)
        {
            var payroll = await _context.EmployeePayrolls
                .FirstOrDefaultAsync(p => p.PayrollId == payrollId);

            if (payroll == null)
                throw new RootPayrollNotFoundEx(payrollId);

            if (payroll.Status != "Approved")
                throw new EmployeeValidationException(
                    "Payroll must be approved before marking as paid.");

            payroll.Status = "Paid";
            payroll.PaymentDate = paymentDate;
            payroll.UpdatedBy = updatedByUserId;
            payroll.UpdatedDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();
        }
    }
}