using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    public interface IDepartmentRepository
    {
        // Query Methods
        Task<Department?> GetDepartmentByIdAsync(
            int departmentId,
            CancellationToken cancellationToken = default);

        Task<Department?> GetDepartmentByNameAsync(
            string departmentName,
            int branchId,
            CancellationToken cancellationToken = default);

        Task<Department?> GetDepartmentByCodeAsync(
            string departmentCode,
            int branchId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Department>> GetDepartmentsByBranchAsync(
            int branchId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Department>> GetActiveDepartmentsByBranchAsync(
            int branchId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Department>> GetDepartmentsByCompanyAsync(
            int companyId,
            CancellationToken cancellationToken = default);

        Task<PagedResult<Department>> SearchDepartmentsAsync(
            string? searchTerm,
            int? branchId,
            int? companyId,
            bool? isActive,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDescending,
            CancellationToken cancellationToken = default);

        // Validation Methods
        Task<bool> IsDepartmentNameUniqueAsync(
            string departmentName,
            int branchId,
            int? excludingDepartmentId = null,
            CancellationToken cancellationToken = default);

        Task<bool> IsDepartmentCodeUniqueAsync(
            string departmentCode,
            int branchId,
            int? excludingDepartmentId = null,
            CancellationToken cancellationToken = default);

        Task<bool> HasActiveDependenciesAsync(
            int departmentId,
            CancellationToken cancellationToken = default);

        Task<bool> BranchExistsAsync(
            int branchId,
            CancellationToken cancellationToken = default);

        Task<bool> IsBranchActiveAsync(
            int branchId,
            CancellationToken cancellationToken = default);

        Task<bool> CompanyExistsAsync(
            int companyId,
            CancellationToken cancellationToken = default);

        // NEW: Get CompanyId from selected Branch
        Task<int?> GetCompanyIdByBranchAsync(
            int branchId,
            CancellationToken cancellationToken = default);

        // Command Methods
        Task AddDepartmentAsync(
            Department department,
            CancellationToken cancellationToken = default);

        void UpdateDepartment(Department department);

        void RemoveDepartment(Department department);

        // Persistence
        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }
}
