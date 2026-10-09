using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using ERPWMS.Application.DTOs.EmployeeSalary;  
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.EmployeeSalary.GetEmployeeSalaries
{
    public class GetEmployeeSalariesQueryHandler : IRequestHandler<GetEmployeeSalariesQuery, GetEmployeeSalariesResult>
    {
        private readonly IEmployeeSalaryRepository _employeeSalaryRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public GetEmployeeSalariesQueryHandler(
            IEmployeeSalaryRepository employeeSalaryRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _employeeSalaryRepository = employeeSalaryRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<GetEmployeeSalariesResult> Handle(GetEmployeeSalariesQuery request, CancellationToken cancellationToken)
        {
            // Validate: Check if employee exists
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                throw new EmployeeNotFoundException(request.EmployeeId);
            }

            // Get salaries
            var salaries = await _employeeSalaryRepository.GetPagedByEmployeeIdAsync(
                request.EmployeeId,
                request.Page,
                request.PageSize);

            // Filter if needed
            if (!request.IncludeInactive)
            {
                salaries = salaries.Where(s => s.IsActive == true);
            }

            var totalCount = await _employeeSalaryRepository.GetCountByEmployeeIdAsync(request.EmployeeId);

            var result = new GetEmployeeSalariesResult
            {
                Salaries = _mapper.Map<List<EmployeeSalaryListDto>>(salaries),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return result;
        }
    }
}
