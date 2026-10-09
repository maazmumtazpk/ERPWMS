using MediatR;
using ERPWMS.Application.DTOs.EmployeeSalary;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.EmployeeSalary.GetEmployeeSalaryDetails
{
    public class GetEmployeeSalaryDetailsQuery : IRequest<EmployeeSalaryDto>
    {
        public int SalaryId { get; set; }
        public int EmployeeId { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }
}