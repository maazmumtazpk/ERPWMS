using MediatR;
using ERPWMS.Application.DTOs.EmployeeSalary;
namespace ERPWMS.Application.Commands.EmployeeSalary.ChangeEmployeeSalary
{
    public class ChangeEmployeeSalaryCommand : IRequest<EmployeeSalaryDto>
    {
        public int SalaryId { get; set; }
        public int EmployeeId { get; set; }
        public CreateEmployeeSalaryRequest Request { get; set; } = null!;
        public int CurrentUserId { get; set; }
    }
}
