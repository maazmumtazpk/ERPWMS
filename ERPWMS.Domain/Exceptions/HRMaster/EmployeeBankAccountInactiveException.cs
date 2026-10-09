using System;

namespace ERPWMS.Domain.Exceptions.HRMaster
{
    public class EmployeeBankAccountInactiveException : Exception
    {
        public EmployeeBankAccountInactiveException() : base("Employee bank account is inactive.") { }
        public EmployeeBankAccountInactiveException(string message) : base(message) { }
        public EmployeeBankAccountInactiveException(string message, Exception inner) : base(message, inner) { }
    }
}