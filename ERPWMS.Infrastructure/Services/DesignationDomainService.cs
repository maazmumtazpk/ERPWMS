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
    public class DesignationDomainService : IDesignationDomainService
    {
        private readonly IDesignationRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILoggingService _logger;

        public DesignationDomainService(
            IDesignationRepository repository,
            IDateTimeProvider dateTimeProvider,
            ILoggingService logger)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task<Designation> RegisterDesignationAsync(
            string designationName, int? rankLevel, int departmentId,
            string createdBy, CancellationToken cancellationToken = default)
        {
            DesignationBusinessRules.ValidateDesignationName(designationName);

            var designation = new Designation
            {
                DesignationName = designationName,
                RankLevel = rankLevel,
                DepartmentId = departmentId,
                IsActive = true,
                IsDeleted = false,
                CreatedDate = _dateTimeProvider.Now,
                CreatedBy = createdBy.ToString()
            };

            await _repository.AddDesignationAsync(designation, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return designation;
        }

        public async Task<Designation> UpdateDesignationProfileAsync(
            int designationId, string designationName, int? rankLevel,
            string updatedBy, CancellationToken cancellationToken = default)
        {
            var designation = await GetValidatedDesignationByIdAsync(designationId, cancellationToken);

            designation.DesignationName = designationName;
            designation.RankLevel = rankLevel;
            designation.UpdatedDate = _dateTimeProvider.Now;
            designation.UpdatedBy = updatedBy.ToString();

            _repository.UpdateDesignation(designation);
            await _repository.SaveChangesAsync(cancellationToken);
            return designation;
        }

        public async Task<Designation> ActivateDesignationAsync(
            int designationId, string activatedBy,
            CancellationToken cancellationToken = default)
        {
            var designation = await GetValidatedDesignationByIdAsync(designationId, cancellationToken);

            designation.IsActive = true;
            designation.UpdatedDate = _dateTimeProvider.Now;
            designation.UpdatedBy = activatedBy;

            _repository.UpdateDesignation(designation);
            await _repository.SaveChangesAsync(cancellationToken);
            return designation;
        }

        public async Task<Designation> DeactivateDesignationAsync(
            int designationId, string deactivatedBy,
            CancellationToken cancellationToken = default)
        {
            var designation = await GetValidatedDesignationByIdAsync(designationId, cancellationToken);

            designation.IsActive = false;
            designation.UpdatedDate = _dateTimeProvider.Now;
            designation.UpdatedBy = deactivatedBy;

            _repository.UpdateDesignation(designation);
            await _repository.SaveChangesAsync(cancellationToken);
            return designation;
        }

        public async Task ValidateDesignationAsync(
            Designation designation, CancellationToken cancellationToken = default)
        {
            if (designation == null) throw new ArgumentNullException(nameof(designation));
            DesignationBusinessRules.ValidateDesignationName(designation.DesignationName);
            DesignationBusinessRules.ValidateNotDeleted(designation);
            await Task.CompletedTask;
        }

        public async Task<Designation> GetValidatedDesignationByIdAsync(
            int designationId, CancellationToken cancellationToken = default)
        {
            var designation = await _repository.GetDesignationByIdAsync(designationId, cancellationToken);
            if (designation == null)
                throw new Domain.Exceptions.DesignationNotFoundException(designationId);
            DesignationBusinessRules.ValidateNotDeleted(designation);
            return designation;
        }

        public async Task<Designation> GetValidatedDesignationByIdAsync(
            int designationId, int departmentId,
            CancellationToken cancellationToken = default)
        {
            var designation = await _repository.GetDesignationByIdAsync(designationId, cancellationToken);
            if (designation == null)
                throw new Domain.Exceptions.DesignationNotFoundException(designationId);

            if (designation.DepartmentId != departmentId)
                throw new Domain.Exceptions.DesignationNotFoundException(designationId, departmentId);

            DesignationBusinessRules.ValidateNotDeleted(designation);
            return designation;
        }
    }
}