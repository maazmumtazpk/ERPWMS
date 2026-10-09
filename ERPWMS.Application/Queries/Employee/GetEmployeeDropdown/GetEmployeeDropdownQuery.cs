using MediatR;
using System.Collections.Generic;
using ERPWMS.Application.DTOs.Employee;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Employee.GetEmployeeDropdown
{
    public class GetEmployeeDropdownQuery : IRequest<List<EmployeeDropdownDto>>
    {
        public bool IncludeInactive { get; set; } = false;
        public int? DepartmentId { get; set; }
        public string? SearchTerm { get; set; }
    }
}
