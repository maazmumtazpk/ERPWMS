using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ERPWMS.Application.DTOs.Employee;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;
using AutoMapper;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Employee.GetEmployeeDetails
{
    public class GetEmployeeDetailsQueryHandler : IRequestHandler<GetEmployeeDetailsQuery, EmployeeDto>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public GetEmployeeDetailsQueryHandler(
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeDto> Handle(GetEmployeeDetailsQuery request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetEmployeeWithDetailsAsync(request.EmployeeId);

            if (employee == null)
            {
                throw new EmployeeNotFoundException(request.EmployeeId);
            }

            if (!request.IncludeDeleted && employee.IsDeleted == true)
            {
                throw new EmployeeNotFoundException(request.EmployeeId);
            }

            var result = _mapper.Map<EmployeeDto>(employee);
            return result;
        }
    }
}

