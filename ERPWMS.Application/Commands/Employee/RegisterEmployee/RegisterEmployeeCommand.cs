using MediatR;
using ERPWMS.Application.DTOs.Employee;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Employee.RegisterEmployee
{
    public class RegisterEmployeeCommand : IRequest<EmployeeDto>
    {
        public RegisterEmployeeRequest Request { get; set; } = null!;
        public int CurrentUserId { get; set; }
    }
}
