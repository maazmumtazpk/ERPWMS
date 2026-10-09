using MediatR;
using System.Collections.Generic;
using ERPWMS.Application.DTOs.Employee;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Employee.GetEmployees
{
    public class GetEmployeesQuery : IRequest<GetEmployeesResult>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeDeleted { get; set; } = false;
    }

    public class GetEmployeesResult
    {
        public List<EmployeeListDto> Employees { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}
