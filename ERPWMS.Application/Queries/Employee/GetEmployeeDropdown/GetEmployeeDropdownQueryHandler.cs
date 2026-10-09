using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ERPWMS.Application.DTOs.Employee;
using ERPWMS.Domain.Interfaces;
using AutoMapper;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Employee.GetEmployeeDropdown
{
    public class GetEmployeeDropdownQueryHandler : IRequestHandler<GetEmployeeDropdownQuery, List<EmployeeDropdownDto>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public GetEmployeeDropdownQueryHandler(
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<List<EmployeeDropdownDto>> Handle(GetEmployeeDropdownQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<ERPWMS.Database.Models.Employee> employees;

            if (request.IncludeInactive)
            {
                employees = await _employeeRepository.GetAllAsync();
            }
            else
            {
                employees = await _employeeRepository.GetActiveEmployeesAsync();
            }

            if (request.DepartmentId.HasValue)
            {
                employees = employees.Where(e => e.DepartmentId == request.DepartmentId.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLower();
                employees = employees.Where(e =>
                    e.EmployeeCode.ToLower().Contains(term) ||
                    e.FirstName.ToLower().Contains(term) ||
                    e.LastName.ToLower().Contains(term) ||
                    (e.FirstName + " " + e.LastName).ToLower().Contains(term));
            }

            var result = _mapper.Map<List<EmployeeDropdownDto>>(employees);
            return result.OrderBy(e => e.FullName).ToList();
        }
    }
}

