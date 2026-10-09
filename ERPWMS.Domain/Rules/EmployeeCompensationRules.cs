using System;
using ERPWMS.Domain.Exceptions.Employee;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Rules
{
    public static class EmployeeCompensationRules
    {
        public static void ValidateSalaryComponents(
            decimal basicSalary,
            decimal? houseRent,
            decimal? utilityAllowance,
            decimal? medicalAllowance,
            decimal? otherAllowances,
            decimal? incomeTax,
            decimal? professionalTax,
            decimal? otherDeductions)
        {
            if (basicSalary <= 0)
                throw new EmployeeCompensationException(
                    "Basic salary must be greater than zero.");

            if (houseRent.HasValue && houseRent < 0)
                throw new EmployeeCompensationException(
                    "House rent cannot be negative.");

            if (utilityAllowance.HasValue && utilityAllowance < 0)
                throw new EmployeeCompensationException(
                    "Utility allowance cannot be negative.");

            if (medicalAllowance.HasValue && medicalAllowance < 0)
                throw new EmployeeCompensationException(
                    "Medical allowance cannot be negative.");

            if (otherAllowances.HasValue && otherAllowances < 0)
                throw new EmployeeCompensationException(
                    "Other allowances cannot be negative.");

            if (incomeTax.HasValue && incomeTax < 0)
                throw new EmployeeCompensationException(
                    "Income tax cannot be negative.");

            if (professionalTax.HasValue && professionalTax < 0)
                throw new EmployeeCompensationException(
                    "Professional tax cannot be negative.");

            if (otherDeductions.HasValue && otherDeductions < 0)
                throw new EmployeeCompensationException(
                    "Other deductions cannot be negative.");
        }

        public static void ValidatePayrollValues(
            decimal grossPay,
            decimal totalDeductions,
            decimal netPay)
        {
            if (grossPay < 0)
                throw new EmployeeCompensationException(
                    "Gross pay cannot be negative.");

            if (totalDeductions < 0)
                throw new EmployeeCompensationException(
                    "Total deductions cannot be negative.");

            if (netPay != grossPay - totalDeductions)
                throw new EmployeeCompensationException(
                    "Net pay must equal gross pay minus total deductions.");
        }

        public static void ValidatePayrollMonth(DateOnly monthYear)
        {
            if (monthYear.Day != 1)
                throw new EmployeeCompensationException(
                    "Payroll month must be the first day of the month.");

            if (monthYear > DateOnly.FromDateTime(DateTime.Today))
                throw new EmployeeCompensationException(
                    "Payroll month cannot be in the future.");
        }
    }
}