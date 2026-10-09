using System;

namespace ERPWMS.Domain.Exceptions.HRMaster
{
    public class EmployeeBankAccountValidationException : Exception
    {
        public EmployeeBankAccountValidationException() : base("Employee bank account validation failed.") { }
        public EmployeeBankAccountValidationException(string message) : base(message) { }
        public EmployeeBankAccountValidationException(string message, Exception inner) : base(message, inner) { }
    }
}