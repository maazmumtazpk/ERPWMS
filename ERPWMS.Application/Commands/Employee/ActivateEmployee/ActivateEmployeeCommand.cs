using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Employee.ActivateEmployee
{
    public class ActivateEmployeeCommand : IRequest<bool>
    {
        public int EmployeeId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
