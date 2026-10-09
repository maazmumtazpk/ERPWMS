using MediatR;
using System.Collections.Generic;
using ERPWMS.Application.DTOs.EmployeeSalary;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.EmployeeSalary.GetEmployeeSalaries
{
    public class GetEmployeeSalariesQuery : IRequest<GetEmployeeSalariesResult>
    {
        public int EmployeeId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeInactive { get; set; } = false;
    }

    public class GetEmployeeSalariesResult
    {
        public List<EmployeeSalaryListDto> Salaries { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}