using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using ERPWMS.Application.DTOs.EmployeeSalary;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.EmployeeSalary.GetEmployeeSalaryDetails
{
    public class GetEmployeeSalaryDetailsQueryHandler : IRequestHandler<GetEmployeeSalaryDetailsQuery, EmployeeSalaryDto>
    {
        private readonly IEmployeeSalaryRepository _employeeSalaryRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public GetEmployeeSalaryDetailsQueryHandler(
            IEmployeeSalaryRepository employeeSalaryRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _employeeSalaryRepository = employeeSalaryRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeSalaryDto> Handle(GetEmployeeSalaryDetailsQuery request, CancellationToken cancellationToken)
        {
            // Validate: Check if employee exists
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                throw new EmployeeNotFoundException(request.EmployeeId);
            }

            // Get salary with details
            var salary = await _employeeSalaryRepository.GetByIdWithDetailsAsync(request.SalaryId);
            if (salary == null)
            {
                throw new EmployeeSalaryNotFoundException(request.SalaryId);
            }

            // Validate: Check if salary belongs to employee
            if (salary.EmployeeId != request.EmployeeId)
            {
                throw new EmployeeSalaryNotFoundException(request.SalaryId, request.EmployeeId);
            }

            // Check if inactive and not allowed
            if (!request.IncludeInactive && salary.IsActive != true)
            {
                throw new EmployeeSalaryNotFoundException(request.SalaryId);
            }

            var result = _mapper.Map<EmployeeSalaryDto>(salary);
            return result;
        }
    }
}
