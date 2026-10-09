using System;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Services
{
    public interface IEmployeeDomainService
    {
        Task EnsureUniqueCodeAsync(string code, int? excludeId = null);
        Task EnsureUniqueCnicAsync(string? cnic, int? excludeId = null);
        Task<Employee> RegisterEmployeeAsync(Employee employee);
        Task DeactivateEmployeeAsync(int employeeId, string? reason, DateOnly? terminationDate, string updatedByUserId);
        Task ActivateEmployeeAsync(int employeeId, string updatedByUserId);
        Task<EmployeeSalary> AssignSalaryStructureAsync(
            int employeeId, decimal basicSalary, decimal? houseRent,
            decimal? utilityAllowance, decimal? medicalAllowance,
            decimal? otherAllowances, decimal? incomeTax,
            decimal? professionalTax, decimal? otherDeductions,
            DateOnly effectiveDate, string createdByUserId);
        Task<EmployeePayroll> GeneratePayrollAsync(int employeeId, DateOnly monthYear, string createdByUserId);
        Task ApprovePayrollAsync(int payrollId, int approvedByUserId);
        Task MarkPayrollPaidAsync(int payrollId, DateTime paymentDate, string updatedByUserId);
    }
}



