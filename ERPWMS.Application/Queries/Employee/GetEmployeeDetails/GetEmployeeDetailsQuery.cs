using MediatR;
using ERPWMS.Application.DTOs.Employee;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Employee.GetEmployeeDetails
{
    public class GetEmployeeDetailsQuery : IRequest<EmployeeDto>
    {
        public int EmployeeId { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}
