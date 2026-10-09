using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Employee.DeactivateEmployee
{
    public class DeactivateEmployeeCommand : IRequest<bool>
    {
        public int EmployeeId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
