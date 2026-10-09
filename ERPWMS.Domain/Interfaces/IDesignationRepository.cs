using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    public interface IDesignationRepository
    {
        // Query Methods
        Task<Designation?> GetDesignationByIdAsync(int designationId, CancellationToken cancellationToken = default);
        Task<Designation?> GetDesignationByNameAsync(string designationName, int departmentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Designation>> GetDesignationsByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Designation>> GetActiveDesignationsByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default);
        Task<IEnumerable<Designation>> GetDesignationsByCompanyAsync(int companyId, CancellationToken cancellationToken = default);
        Task<PagedResult<Designation>> SearchDesignationsAsync(
            string? searchTerm,
            int? departmentId,
            int? companyId,
            bool? isActive,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDescending,
            CancellationToken cancellationToken = default);

        // Validation Methods
        Task<bool> IsDesignationNameUniqueAsync(string designationName, int departmentId, int? excludingDesignationId = null, CancellationToken cancellationToken = default);
        Task<bool> HasActiveDependenciesAsync(int designationId, CancellationToken cancellationToken = default);
        Task<bool> DepartmentExistsAsync(int departmentId, CancellationToken cancellationToken = default);
        Task<bool> IsDepartmentActiveAsync(int departmentId, CancellationToken cancellationToken = default);
        Task<bool> CompanyExistsAsync(int companyId, CancellationToken cancellationToken = default);

        // Command Methods
        Task AddDesignationAsync(Designation designation, CancellationToken cancellationToken = default);
        void UpdateDesignation(Designation designation);
        void RemoveDesignation(Designation designation);

        // Persistence
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    
}

