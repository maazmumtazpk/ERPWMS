using System;

namespace ERPWMS.Domain.Exceptions
{
    /// <summary>
    /// EmployeeBankAccountNotFoundException — Auto-generated exception class
    /// </summary>
    public class EmployeeBankAccountNotFoundException : DomainException
    {
        public EmployeeBankAccountNotFoundException(string message) : base(message) { }
        
        public EmployeeBankAccountNotFoundException(string message, Exception innerException) 
            : base(message, innerException) { }
        
        public EmployeeBankAccountNotFoundException() : base("EmployeeBankAccountNotFoundException occurred.") { }
    }
}