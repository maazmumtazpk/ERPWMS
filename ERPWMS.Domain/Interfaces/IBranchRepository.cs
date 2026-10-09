using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Branch aggregate root
    /// </summary>
    public interface IBranchRepository
    {
        // Query Methods
        Task<Branch?> GetBranchByIdAsync(int branchId, CancellationToken cancellationToken = default);
        Task<Branch?> GetBranchByNameAsync(string branchName, int companyId, CancellationToken cancellationToken = default);
        Task<Branch?> GetBranchByCodeAsync(string branchCode, int companyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Branch>> GetBranchesByCompanyAsync(int companyId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Branch>> GetActiveBranchesByCompanyAsync(int companyId, CancellationToken cancellationToken = default);
        Task<PagedResult<Branch>> SearchBranchesAsync(
            string? searchTerm,
            int? companyId,
            bool? isActive,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDescending,
            CancellationToken cancellationToken = default);

        // NEW METHOD - Add this line
        Task<IEnumerable<Branch>> GetAllActiveBranchesAsync(CancellationToken cancellationToken = default);

        // Validation Methods
        Task<bool> IsBranchNameUniqueAsync(string branchName, int companyId, int? excludingBranchId = null, CancellationToken cancellationToken = default);
        Task<bool> IsBranchCodeUniqueAsync(string branchCode, int companyId, int? excludingBranchId = null, CancellationToken cancellationToken = default);
        Task<bool> HasActiveDependenciesAsync(int branchId, CancellationToken cancellationToken = default);
        Task<bool> CompanyExistsAsync(int companyId, CancellationToken cancellationToken = default);
        Task<bool> IsCompanyActiveAsync(int companyId, CancellationToken cancellationToken = default);

        // Command Methods
        Task AddBranchAsync(Branch branch, CancellationToken cancellationToken = default);
        void UpdateBranch(Branch branch);
        void RemoveBranch(Branch branch);

        // Persistence
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
