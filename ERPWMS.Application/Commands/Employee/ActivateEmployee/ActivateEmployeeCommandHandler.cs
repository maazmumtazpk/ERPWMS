using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Employee.ActivateEmployee
{
    public class ActivateEmployeeCommandHandler : IRequestHandler<ActivateEmployeeCommand, bool>
    {
        private readonly IEmployeeRepository _employeeRepository;

        public ActivateEmployeeCommandHandler(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public async Task<bool> Handle(ActivateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);

            if (employee == null)
            {
                throw new EmployeeNotFoundException(request.EmployeeId);
            }

            if (employee.IsDeleted == true)
            {
                throw new EmployeeDeletedException(request.EmployeeId);
            }

            if (employee.IsActive == true)
            {
                return true;
            }

            employee.IsActive = true;
            employee.UpdatedBy = request.CurrentUserId.ToString();
            employee.UpdatedDate = DateTime.UtcNow;

            _employeeRepository.Update(employee);
            await _employeeRepository.SaveChangesAsync();

            return true;
        }
    }
}

