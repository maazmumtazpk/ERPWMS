using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Services
{
    public interface IDesignationDomainService
    {
        Task<Designation> RegisterDesignationAsync(
            string designationName,
            int? rankLevel,
            int departmentId,
            string createdBy,
            CancellationToken cancellationToken = default);

        Task<Designation> UpdateDesignationProfileAsync(
            int designationId,
            string designationName,
            int? rankLevel,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Designation> ActivateDesignationAsync(int designationId, string activatedBy, CancellationToken cancellationToken = default);

        Task<Designation> DeactivateDesignationAsync(int designationId, string deactivatedBy, CancellationToken cancellationToken = default);

        Task ValidateDesignationAsync(Designation designation, CancellationToken cancellationToken = default);

        Task<Designation> GetValidatedDesignationByIdAsync(int designationId, CancellationToken cancellationToken = default);

        Task<Designation> GetValidatedDesignationByIdAsync(int designationId, int departmentId, CancellationToken cancellationToken = default);
    }
}


