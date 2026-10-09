using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;      // ✅ IPayrollDomainService ke liye
using ERPWMS.Database.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services   // ✅ Folder ke saath match
{
    // ✅ FIX: Primary Constructor
    public class PayrollDomainService(IEmployeeRepository repository) : IPayrollDomainService
    {
        public async Task<EmployeePayroll> GenerateAsync(
            int employeeId,
            DateOnly monthYear,
            string createdByUserId,   // ✅ FIX: lowercase (body se match)
            CancellationToken cancellationToken = default)
        {
            var employee = await repository.GetByIdAsync(employeeId);

            if (employee is null)   // ✅ simplified null check
                throw new PayrollDomainException("Employee not found.");

            if (employee.IsActive != true)
                throw new PayrollDomainException(
                    "Payroll cannot be generated for an inactive employee.");

            var existing = await repository.GetPayrollByPeriodAsync(employeeId, monthYear);

            if (existing is not null)
                throw new PayrollDomainException(
                    "Payroll already exists for this employee and month.");

            var salary = await repository.GetSalaryStructureByDateAsync(employeeId, monthYear);

            if (salary is null)
                throw new PayrollDomainException(
                    "No active salary structure exists for this employee.");

            decimal basic = salary.BasicSalary;

            decimal allowances =
                (salary.HouseRent ?? 0) +
                (salary.UtilityAllowance ?? 0) +
                (salary.MedicalAllowance ?? 0) +
                (salary.OtherAllowances ?? 0);

            decimal deductions =
                (salary.IncomeTax ?? 0) +
                (salary.ProfessionalTax ?? 0) +
                (salary.OtherDeductions ?? 0);

            decimal grossPay = basic + allowances;
            decimal netPay = grossPay - deductions;

            if (netPay < 0)
                throw new PayrollDomainException("Net salary cannot be negative.");

            var payroll = new EmployeePayroll
            {
                SalaryId = salary.SalaryId,
                EmployeeId = employeeId,
                MonthYear = monthYear,
                GrossPay = grossPay,
                TotalDeductions = deductions,
                NetPay = netPay,
                Status = "Draft",
                CreatedBy = createdByUserId,   // ✅ already string
                CreatedDate = DateTime.UtcNow
            };

            await repository.AddPayrollAsync(payroll);
            await repository.SaveAsync();

            return payroll;
        }

        public async Task ApproveAsync(
            int payrollId,
            int approvedByUserId,
            CancellationToken cancellationToken = default)
        {
            var payroll = await repository.GetPayrollByIdAsync(payrollId);

            if (payroll is null)   // ✅ simplified
                throw new PayrollDomainException("Payroll not found.");

            if (payroll.Status != "Submitted")
                throw new PayrollDomainException("Only submitted payroll can be approved.");

            payroll.Status = "Approved";
            payroll.ApprovedBy = approvedByUserId.ToString();   // ✅ int -> string
            payroll.ApprovedDate = DateTime.UtcNow;
            payroll.UpdatedBy = approvedByUserId.ToString();    // ✅ int -> string
            payroll.UpdatedDate = DateTime.UtcNow;

            await repository.UpdatePayrollAsync(payroll);
            await repository.SaveAsync();
        }

        public async Task MarkPaidAsync(
            int payrollId,
            DateTime paymentDate,
            int updatedByUserId,
            CancellationToken cancellationToken = default)
        {
            var payroll = await repository.GetPayrollByIdAsync(payrollId);

            if (payroll is null)   // ✅ simplified
                throw new PayrollDomainException("Payroll not found.");

            if (payroll.Status != "Approved")
                throw new PayrollDomainException("Only approved payroll can be marked as paid.");

            payroll.Status = "Paid";
            payroll.PaymentDate = paymentDate;
            payroll.UpdatedBy = updatedByUserId.ToString();   // ✅ int -> string
            payroll.UpdatedDate = DateTime.UtcNow;

            await repository.UpdatePayrollAsync(payroll);
            await repository.SaveAsync();
        }
    }
}