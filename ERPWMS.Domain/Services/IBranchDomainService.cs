using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Services
{
    /// <summary>
    /// Domain service for Branch business logic
    /// </summary>
    public interface IBranchDomainService
    {
        /// <summary>
        /// Registers a new branch with validation
        /// </summary>
        Task<Branch> RegisterBranchAsync(
            string branchName,
            string? branchCode,
            string? address,
            string? phone,
            string? email,
            int companyId,
            string createdBy,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates branch profile with validation
        /// </summary>
        Task<Branch> UpdateBranchProfileAsync(
            int branchId,
            string branchName,
            string? branchCode,
            string? address,
            string? phone,
            string? email,
            string updatedBy,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Activates a branch
        /// </summary>
        Task<Branch> ActivateBranchAsync(int branchId, string activatedBy, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deactivates a branch with dependency check
        /// </summary>
        Task<Branch> DeactivateBranchAsync(int branchId, string deactivatedBy, CancellationToken cancellationToken = default);

        /// <summary>
        /// Validates branch business rules
        /// </summary>
        Task ValidateBranchAsync(Branch branch, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a branch by ID with validation
        /// </summary>
        Task<Branch> GetValidatedBranchByIdAsync(int branchId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a branch by ID with company validation
        /// </summary>
        Task<Branch> GetValidatedBranchByIdAsync(int branchId, int companyId, CancellationToken cancellationToken = default);
    }
}
