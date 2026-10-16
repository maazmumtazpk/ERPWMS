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
    public class BranchDomainService : IBranchDomainService
    {
        private readonly IBranchRepository _repository;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ILoggingService _logger;

        public BranchDomainService(
            IBranchRepository repository,
            IDateTimeProvider dateTimeProvider,
            ILoggingService logger)
        {
            _repository = repository;
            _dateTimeProvider = dateTimeProvider;
            _logger = logger;
        }

        public async Task<Branch> RegisterBranchAsync(
            string branchName, string? branchCode, string? address,
            string? phone, string? email, int companyId, string createdBy,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Registering branch: {branchName}");
            BranchBusinessRules.ValidateBranchName(branchName);

            var branch = new Branch
            {
                BranchName = branchName,
                BranchCode = branchCode,
                Address = address,
                Phone = phone,
                Email = email,
                CompanyId = companyId,
                IsActive = true,
                IsDeleted = false,
                CreatedDate = _dateTimeProvider.Now,
                CreatedBy = createdBy
            };

            await _repository.AddBranchAsync(branch, cancellationToken);
            await _repository.SaveChangesAsync(cancellationToken);
            return branch;
        }

        public async Task<Branch> UpdateBranchProfileAsync(
            int branchId, string branchName, string? branchCode,
            string? address, string? phone, string? email,
            string updatedBy, CancellationToken cancellationToken = default)
        {
            var branch = await GetValidatedBranchByIdAsync(branchId, cancellationToken);

            branch.BranchName = branchName;
            branch.BranchCode = branchCode;
            branch.Address = address;
            branch.Phone = phone;
            branch.Email = email;
            branch.UpdatedBy = updatedBy;
            branch.UpdatedDate = _dateTimeProvider.Now;

            _repository.UpdateBranch(branch);
            await _repository.SaveChangesAsync(cancellationToken);
            return branch;
        }

        public async Task<Branch> ActivateBranchAsync(
            int branchId, string activatedBy,
            CancellationToken cancellationToken = default)
        {
            var branch = await GetValidatedBranchByIdAsync(branchId, cancellationToken);
            branch.IsActive = true;
            branch.UpdatedBy = activatedBy;
            branch.UpdatedDate = _dateTimeProvider.Now;
            _repository.UpdateBranch(branch);
            await _repository.SaveChangesAsync(cancellationToken);
            return branch;
        }

        public async Task<Branch> DeactivateBranchAsync(
            int branchId, string deactivatedBy,
            CancellationToken cancellationToken = default)
        {
            var branch = await GetValidatedBranchByIdAsync(branchId, cancellationToken);
            branch.IsActive = false;
            branch.UpdatedBy = deactivatedBy;
            branch.UpdatedDate = _dateTimeProvider.Now;
            _repository.UpdateBranch(branch);
            await _repository.SaveChangesAsync(cancellationToken);
            return branch;
        }

        public async Task ValidateBranchAsync(
            Branch branch, CancellationToken cancellationToken = default)
        {
            if (branch == null) throw new ArgumentNullException(nameof(branch));
            BranchBusinessRules.ValidateBranchName(branch.BranchName);
            BranchBusinessRules.ValidateNotDeleted(branch);
            await Task.CompletedTask;
        }

        public async Task<Branch> GetValidatedBranchByIdAsync(
            int branchId, CancellationToken cancellationToken = default)
        {
            var branch = await _repository.GetBranchByIdAsync(branchId, cancellationToken);
            if (branch == null)
                throw new Domain.Exceptions.BranchNotFoundException(branchId);
            BranchBusinessRules.ValidateNotDeleted(branch);
            return branch;
        }

        public async Task<Branch> GetValidatedBranchByIdAsync(
            int branchId, int companyId,
            CancellationToken cancellationToken = default)
        {
            var branch = await _repository.GetBranchByIdAsync(branchId, cancellationToken);
            if (branch == null)
                throw new Domain.Exceptions.BranchNotFoundException(branchId);
            if (branch.CompanyId != companyId)
                throw new Domain.Exceptions.BranchNotFoundException(branchId, companyId);
            BranchBusinessRules.ValidateNotDeleted(branch);
            return branch;
        }
    }
}
