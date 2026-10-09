using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Services
{
    public interface IDepartmentDomainService
    {
        Task<Department> RegisterDepartmentAsync(
            string departmentName,
            string? departmentCode,
            int branchId,
            string createdBy,
            CancellationToken cancellationToken = default);

        Task<Department> UpdateDepartmentProfileAsync(
            int departmentId,
            string departmentName,
            string? departmentCode,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Department> ActivateDepartmentAsync(
            int departmentId,
            string activatedBy,
            CancellationToken cancellationToken = default);

        Task<Department> DeactivateDepartmentAsync(
            int departmentId,
            string deactivatedBy,
            CancellationToken cancellationToken = default);

        Task ValidateDepartmentAsync(
            Department department,
            CancellationToken cancellationToken = default);

        Task<Department> GetValidatedDepartmentByIdAsync(
            int departmentId,
            CancellationToken cancellationToken = default);

        Task<Department> GetValidatedDepartmentByIdAsync(
            int departmentId,
            int branchId,
            CancellationToken cancellationToken = default);
    }
}
