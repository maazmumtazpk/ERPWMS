using MediatR;
using ERPWMS.Application.DTOs.EmployeeSalary;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.EmployeeSalary.CreateEmployeeSalary
{
    public class CreateEmployeeSalaryCommand : IRequest<EmployeeSalaryDto>
    {
        public CreateEmployeeSalaryRequest Request { get; set; } = null!;
        public int CurrentUserId { get; set; }
    }
}