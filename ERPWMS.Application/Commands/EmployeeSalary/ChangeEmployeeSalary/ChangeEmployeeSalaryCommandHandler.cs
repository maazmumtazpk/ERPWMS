using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using ERPWMS.Application.DTOs.EmployeeSalary;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.EmployeeSalary.ChangeEmployeeSalary
{
    public class ChangeEmployeeSalaryCommandHandler : IRequestHandler<ChangeEmployeeSalaryCommand, EmployeeSalaryDto>
    {
        private readonly IEmployeeSalaryRepository _employeeSalaryRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public ChangeEmployeeSalaryCommandHandler(
            IEmployeeSalaryRepository employeeSalaryRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _employeeSalaryRepository = employeeSalaryRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeSalaryDto> Handle(ChangeEmployeeSalaryCommand request, CancellationToken cancellationToken)
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

            // Validate: Check if salary is inactive
            if (salary.IsActive != true)
            {
                throw new EmployeeSalaryInactiveException(request.SalaryId);
            }

            // Validate: Check if salary already exists for this date (excluding current)
            var exists = await _employeeSalaryRepository.SalaryExistsForEmployeeAndDateAsync(
                request.EmployeeId,
                request.Request.EffectiveDate,
                request.SalaryId);

            if (exists)
            {
                throw new EmployeeSalaryAlreadyExistsException(
                    request.EmployeeId,
                    request.Request.EffectiveDate);
            }

            // Validate: Check if effective date is in the future
            if (request.Request.EffectiveDate > DateOnly.FromDateTime(DateTime.UtcNow))
            {
                throw new EmployeeSalaryFutureDateException(request.Request.EffectiveDate);
            }

            // Update salary
            salary.EffectiveDate = request.Request.EffectiveDate;
            salary.BasicSalary = request.Request.BasicSalary;
            salary.HouseRent = request.Request.HouseRent;
            salary.UtilityAllowance = request.Request.UtilityAllowance;
            salary.MedicalAllowance = request.Request.MedicalAllowance;
            salary.OtherAllowances = request.Request.OtherAllowances;
            salary.IncomeTax = request.Request.IncomeTax;
            salary.ProfessionalTax = request.Request.ProfessionalTax;
            salary.OtherDeductions = request.Request.OtherDeductions;
            salary.UpdatedBy = request.CurrentUserId.ToString();
            salary.UpdatedDate = DateTime.UtcNow;

            _employeeSalaryRepository.Update(salary);
            await _employeeSalaryRepository.SaveChangesAsync();

            return _mapper.Map<EmployeeSalaryDto>(salary);
        }
    }
}
