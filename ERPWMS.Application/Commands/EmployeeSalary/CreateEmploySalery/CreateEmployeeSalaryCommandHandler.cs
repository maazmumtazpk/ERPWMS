using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using ERPWMS.Application.DTOs.EmployeeSalary;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.EmployeeSalary.CreateEmployeeSalary
{
    public class CreateEmployeeSalaryCommandHandler
        : IRequestHandler<CreateEmployeeSalaryCommand, EmployeeSalaryDto>
    {
        private readonly IEmployeeSalaryRepository _employeeSalaryRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public CreateEmployeeSalaryCommandHandler(
            IEmployeeSalaryRepository employeeSalaryRepository,
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _employeeSalaryRepository = employeeSalaryRepository;
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeSalaryDto> Handle(
            CreateEmployeeSalaryCommand request,
            CancellationToken cancellationToken)
        {
            // 1. CHECK EMPLOYEE EXISTS
            var employee = await _employeeRepository
                .GetByIdAsync(request.Request.EmployeeId);

            if (employee == null)
            {
                throw new EmployeeNotFoundException(
                    request.Request.EmployeeId);
            }

            // 2. CHECK DUPLICATE SALARY
            var exists =
                await _employeeSalaryRepository
                    .SalaryExistsForEmployeeAndDateAsync(
                        request.Request.EmployeeId,
                        request.Request.EffectiveDate);

            if (exists)
            {
                throw new EmployeeSalaryAlreadyExistsException(
                    request.Request.EmployeeId,
                    request.Request.EffectiveDate);
            }

            // 3. CHECK FUTURE EFFECTIVE DATE
            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (request.Request.EffectiveDate > today)
            {
                throw new EmployeeSalaryFutureDateException(
                    request.Request.EffectiveDate);
            }

            // 4. CALCULATE ALLOWANCES
            decimal totalAllowances =
                (request.Request.HouseRent ?? 0m) +
                (request.Request.UtilityAllowance ?? 0m) +
                (request.Request.MedicalAllowance ?? 0m) +
                (request.Request.OtherAllowances ?? 0m);

            // 5. CALCULATE DEDUCTIONS
            decimal totalDeductions =
                (request.Request.IncomeTax ?? 0m) +
                (request.Request.ProfessionalTax ?? 0m) +
                (request.Request.OtherDeductions ?? 0m);

            // 6. CALCULATE NET SALARY
            decimal netSalary =
                request.Request.BasicSalary +
                totalAllowances -
                totalDeductions;

            // 7. CREATE EMPLOYEE SALARY ENTITY
            var salary =
                new ERPWMS.Database.Models.EmployeeSalary
                {
                    EmployeeId = request.Request.EmployeeId,
                    EffectiveDate = request.Request.EffectiveDate,
                    BasicSalary = request.Request.BasicSalary,
                    HouseRent = request.Request.HouseRent,
                    UtilityAllowance = request.Request.UtilityAllowance,
                    MedicalAllowance = request.Request.MedicalAllowance,
                    OtherAllowances = request.Request.OtherAllowances,
                    TotalAllowances = totalAllowances,
                    IncomeTax = request.Request.IncomeTax,
                    ProfessionalTax = request.Request.ProfessionalTax,
                    OtherDeductions = request.Request.OtherDeductions,
                    TotalDeductions = totalDeductions,
                    NetSalary = netSalary,
                    IsActive = true,

                    // ✅ FIX: int → string
                    CreatedBy = request.CurrentUserId.ToString(),

                    CreatedDate = DateTime.UtcNow
                };

            // 8. SAVE
            await _employeeSalaryRepository.AddAsync(salary);
            await _employeeSalaryRepository.SaveChangesAsync();

            // 9. RETURN DTO
            return _mapper.Map<EmployeeSalaryDto>(salary);
        }
    }
}