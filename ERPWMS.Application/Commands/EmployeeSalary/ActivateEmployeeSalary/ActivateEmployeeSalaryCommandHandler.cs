using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.EmployeeSalary.ActivateEmployeeSalary
{
    public class ActivateEmployeeSalaryCommandHandler : IRequestHandler<ActivateEmployeeSalaryCommand, bool>
    {
        private readonly IEmployeeSalaryRepository _employeeSalaryRepository;
        private readonly IEmployeeRepository _employeeRepository;

        public ActivateEmployeeSalaryCommandHandler(
            IEmployeeSalaryRepository employeeSalaryRepository,
            IEmployeeRepository employeeRepository)
        {
            _employeeSalaryRepository = employeeSalaryRepository;
            _employeeRepository = employeeRepository;
        }

        public async Task<bool> Handle(ActivateEmployeeSalaryCommand request, CancellationToken cancellationToken)
        {
            // Validate: Check if employee exists
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee == null)
            {
                throw new EmployeeNotFoundException(request.EmployeeId);
            }

            // Get existing salary
            var salary = await _employeeSalaryRepository.GetByIdAsync(request.SalaryId);
            if (salary == null)
            {
                throw new EmployeeSalaryNotFoundException(request.SalaryId);
            }

            // Validate: Check if salary belongs to employee
            if (salary.EmployeeId != request.EmployeeId)
            {
                throw new EmployeeSalaryNotFoundException(request.SalaryId, request.EmployeeId);
            }

            // Check if already active
            if (salary.IsActive == true)
            {
                return true;
            }

            // Activate salary
            salary.IsActive = true;
            salary.UpdatedBy = request.CurrentUserId.ToString();
            salary.UpdatedDate = DateTime.UtcNow;

            _employeeSalaryRepository.Update(salary);
            await _employeeSalaryRepository.SaveChangesAsync();

            return true;
        }
    }
}
