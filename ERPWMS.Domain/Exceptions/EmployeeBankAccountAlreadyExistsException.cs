using System;

namespace ERPWMS.Domain.Exceptions
{
    public class EmployeeBankAccountAlreadyExistsException : DomainException
    {
        public EmployeeBankAccountAlreadyExistsException() : base("Employee bank account already exists.") { }
        public EmployeeBankAccountAlreadyExistsException(string message) : base(message) { }
        public EmployeeBankAccountAlreadyExistsException(string message, Exception innerException) 
            : base(message, innerException) { }
    }
}