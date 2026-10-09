using System;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Rules;
using ERPWMS.Domain.Services;
using ERPWMS.Application.Common;

namespace ERPWMS.Infrastructure.Services
{
    public class DepartmentDomainService : IDepartmentDomainService
    {
        private readonly IDepartmentRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILoggingService _logger;

        public DepartmentDomainService(
            IDepartmentRepository repository,
            IDateTimeProvider dateTimeProvider,
            ILoggingService logger)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task<Department> RegisterDepartmentAsync(
            string departmentName, string? departmentCode,
            int branchId, string createdBy,
            CancellationToken cancellationToken = default)
        {
            DepartmentBusinessRules.ValidateDepartmentName(departmentName);

            var department = new Department
            {
                DepartmentName = departmentName,
                DepartmentCode = departmentCode,
                BranchId = branchId,
                IsActive = true,
                IsDeleted = false,
                CreatedDate = _dateTimeProvider.Now,
                CreatedBy = createdBy.ToString()
            };

            await _repository.AddDepartmentAsync(department, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return department;
        }

        public async Task<Department> UpdateDepartmentProfileAsync(
            int departmentId, string departmentName, string? departmentCode,
            string updatedBy, CancellationToken cancellationToken = default)
        {
            var department = await GetValidatedDepartmentByIdAsync(departmentId, cancellationToken);

            department.DepartmentName = departmentName;
            department.DepartmentCode = departmentCode;
            department.UpdatedDate = _dateTimeProvider.Now;
            department.UpdatedBy = updatedBy.ToString();

            _repository.UpdateDepartment(department);
            await _repository.SaveChangesAsync(cancellationToken);
            return department;
        }

        public async Task<Department> ActivateDepartmentAsync(
            int departmentId, string activatedBy,
            CancellationToken cancellationToken = default)
        {
            var department = await GetValidatedDepartmentByIdAsync(departmentId, cancellationToken);

            department.IsActive = true;
            department.UpdatedDate = _dateTimeProvider.Now;
            department.UpdatedBy = activatedBy;

            _repository.UpdateDepartment(department);
            await _repository.SaveChangesAsync(cancellationToken);
            return department;
        }

        public async Task<Department> DeactivateDepartmentAsync(
            int departmentId, string deactivatedBy,
            CancellationToken cancellationToken = default)
        {
            var department = await GetValidatedDepartmentByIdAsync(departmentId, cancellationToken);

            department.IsActive = false;
            department.UpdatedDate = _dateTimeProvider.Now;
            department.UpdatedBy = deactivatedBy;

            _repository.UpdateDepartment(department);
            await _repository.SaveChangesAsync(cancellationToken);
            return department;
        }

        public async Task ValidateDepartmentAsync(
            Department department, CancellationToken cancellationToken = default)
        {
            if (department == null) throw new ArgumentNullException(nameof(department));
            DepartmentBusinessRules.ValidateDepartmentName(department.DepartmentName);
            DepartmentBusinessRules.ValidateNotDeleted(department);
            await Task.CompletedTask;
        }

        public async Task<Department> GetValidatedDepartmentByIdAsync(
            int departmentId, CancellationToken cancellationToken = default)
        {
            var department = await _repository.GetDepartmentByIdAsync(departmentId, cancellationToken);
            if (department == null)
                throw new Domain.Exceptions.DepartmentNotFoundException(departmentId);
            DepartmentBusinessRules.ValidateNotDeleted(department);
            return department;
        }

        public async Task<Department> GetValidatedDepartmentByIdAsync(
            int departmentId, int branchId,
            CancellationToken cancellationToken = default)
        {
            var department = await _repository.GetDepartmentByIdAsync(departmentId, cancellationToken);
            if (department == null)
                throw new Domain.Exceptions.DepartmentNotFoundException(departmentId);

            if (department.BranchId != branchId)
                throw new Domain.Exceptions.DepartmentNotFoundException(departmentId, branchId);

            DepartmentBusinessRules.ValidateNotDeleted(department);
            return department;
        }
    }
}