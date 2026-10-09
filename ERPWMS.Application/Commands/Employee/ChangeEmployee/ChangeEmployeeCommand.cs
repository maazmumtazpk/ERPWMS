using MediatR;
using ERPWMS.Application.DTOs.Employee;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Employee.ChangeEmployee
{
    public class ChangeEmployeeCommand : IRequest<EmployeeDto>
    {
        public int EmployeeId { get; set; }
        public RegisterEmployeeRequest Request { get; set; } = null!;
        public int CurrentUserId { get; set; }
    }
}
