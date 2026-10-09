using System;

namespace ERPWMS.Domain.Exceptions.HRMaster
{
    public class EmployeeBankAccountPrimaryException : Exception
    {
        public EmployeeBankAccountPrimaryException() : base("Cannot deactivate primary bank account.") { }
        public EmployeeBankAccountPrimaryException(string message) : base(message) { }
        public EmployeeBankAccountPrimaryException(string message, Exception inner) : base(message, inner) { }
    }
}