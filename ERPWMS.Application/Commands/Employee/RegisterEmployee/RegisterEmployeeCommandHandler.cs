using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;

using ERPWMS.Application.DTOs.Employee;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;

using EmployeeModel = ERPWMS.Database.Models.Employee;

namespace ERPWMS.Application.Commands.Employee.RegisterEmployee
{
    public sealed class RegisterEmployeeCommandHandler
        : IRequestHandler<RegisterEmployeeCommand, EmployeeDto>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public RegisterEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            ArgumentNullException.ThrowIfNull(employeeRepository);
            ArgumentNullException.ThrowIfNull(mapper);

            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeDto> Handle(
            RegisterEmployeeCommand request,
            CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Request);

            // 1. VALIDATE EMPLOYEE CODE
            if (!string.IsNullOrWhiteSpace(request.Request.EmployeeCode))
            {
                var employeeCode = request.Request.EmployeeCode.Trim();

                var exists =
                    await _employeeRepository.EmployeeCodeExistsAsync(
                        employeeCode);

                if (exists)
                {
                    throw new EmployeeCodeAlreadyExistsException(employeeCode);
                }
            }

            // 2. CREATE EMPLOYEE ENTITY
            var employee = new EmployeeModel
            {
                DepartmentId = request.Request.DepartmentId,
                DesignationId = request.Request.DesignationId,
                BranchId = request.Request.BranchId,
                WarehouseId = request.Request.WarehouseId,
                ShiftId = request.Request.ShiftId,

                EmployeeCode = request.Request.EmployeeCode?.Trim() ?? string.Empty,
                FirstName = request.Request.FirstName?.Trim() ?? string.Empty,
                LastName = request.Request.LastName?.Trim() ?? string.Empty,
                FatherName = request.Request.FatherName?.Trim() ?? string.Empty,
                Cnic = request.Request.Cnic?.Trim() ?? string.Empty,

                DOB = request.Request.DOB,
                Gender = request.Request.Gender,

                HireDate = request.Request.HireDate,
                TerminationDate = request.Request.TerminationDate,

                BaseSalary = request.Request.BaseSalary,

                IsActive = true,
                IsDeleted = false,

                // ✅ FIX: int → string
                CreatedBy = request.CurrentUserId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            await _employeeRepository.AddAsync(employee);
            await _employeeRepository.SaveChangesAsync();

            var result = _mapper.Map<EmployeeDto>(employee);
            return result;
        }
    }
}