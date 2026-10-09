using System;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions.Employee
{
    public class EmployeeCompensationException : Exception
    {
        public EmployeeCompensationException(string message)
            : base(message)
        {
        }

        public EmployeeCompensationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}