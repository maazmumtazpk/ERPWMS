using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ERPWMS.Application.DTOs.Employee;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;
using AutoMapper;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Employee.ChangeEmployee
{
    public class ChangeEmployeeCommandHandler : IRequestHandler<ChangeEmployeeCommand, EmployeeDto>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IMapper _mapper;

        public ChangeEmployeeCommandHandler(
            IEmployeeRepository employeeRepository,
            IMapper mapper)
        {
            _employeeRepository = employeeRepository;
            _mapper = mapper;
        }

        public async Task<EmployeeDto> Handle(ChangeEmployeeCommand request, CancellationToken cancellationToken)
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

            if (employee.IsActive != true)
            {
                throw new EmployeeInactiveException(request.EmployeeId);
            }

            // Validate employee code uniqueness
            if (!string.IsNullOrWhiteSpace(request.Request.EmployeeCode) &&
                request.Request.EmployeeCode != employee.EmployeeCode)
            {
                var exists = await _employeeRepository.EmployeeCodeExistsAsync(
                    request.Request.EmployeeCode, request.EmployeeId);
                if (exists)
                {
                    throw new EmployeeCodeAlreadyExistsException(request.Request.EmployeeCode);
                }
            }

            // Update employee
            employee.DepartmentId = request.Request.DepartmentId;
            employee.DesignationId = request.Request.DesignationId;
            employee.BranchId = request.Request.BranchId;
            employee.WarehouseId = request.Request.WarehouseId;
            employee.ShiftId = request.Request.ShiftId;
            employee.EmployeeCode = request.Request.EmployeeCode;
            employee.FirstName = request.Request.FirstName;
            employee.LastName = request.Request.LastName;
            employee.FatherName = request.Request.FatherName;
            employee.Cnic = request.Request.Cnic;

            // ✅ FIX: String → DateOnly? — .ToString() HATA diya
            employee.DOB = request.Request.DOB;

            employee.Gender = request.Request.Gender;
            employee.HireDate = request.Request.HireDate;
            employee.TerminationDate = request.Request.TerminationDate;
            employee.BaseSalary = request.Request.BaseSalary;

            // ✅ FIX: int → string (agar CurrentUserId int hai)
            employee.UpdatedBy = request.CurrentUserId.ToString();

            employee.UpdatedDate = DateTime.UtcNow;

            _employeeRepository.Update(employee);
            await _employeeRepository.SaveChangesAsync();

            var result = _mapper.Map<EmployeeDto>(employee);
            return result;
        }
    }
}