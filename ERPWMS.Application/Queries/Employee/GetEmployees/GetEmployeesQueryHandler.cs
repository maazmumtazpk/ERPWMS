using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ERPWMS.Application.DTOs.Employee;
using ERPWMS.Domain.Interfaces;
using AutoMapper;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Employee.GetEmployees
{
    public class GetEmployeesQueryHandler : IRequestHandler<GetEmployeesQuery, GetEmployeesResult>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public GetEmployeesQueryHandler(
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<GetEmployeesResult> Handle(GetEmployeesQuery request, CancellationToken cancellationToken)
        {
            var employees = await _employeeRepository.GetPagedEmployeesAsync(
                request.Page,
                request.PageSize,
                null,
                request.IncludeDeleted);

            var totalCount = await _employeeRepository.GetTotalCountAsync(null, request.IncludeDeleted);

            var result = new GetEmployeesResult
            {
                Employees = _mapper.Map<List<EmployeeListDto>>(employees),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return result;
        }
    }
}

