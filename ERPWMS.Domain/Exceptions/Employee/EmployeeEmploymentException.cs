using ERPWMS.Domain.Exceptions;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Exceptions.Employee
{
    public class EmployeeEmploymentException : EmployeeDomainException
    {
        public EmployeeEmploymentException(string message)
            : base(message)
        {
        }
    }
}