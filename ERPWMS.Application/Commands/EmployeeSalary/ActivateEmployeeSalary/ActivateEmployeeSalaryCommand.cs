using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.EmployeeSalary.ActivateEmployeeSalary
{
    public class ActivateEmployeeSalaryCommand : IRequest<bool>
    {
        public int SalaryId { get; set; }
        public int EmployeeId { get; set; }
        public int CurrentUserId { get; set; }
    }
}