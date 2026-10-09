using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Services
{
    public interface ICompanyDomainService
    {
        Task<Company> RegisterCompanyAsync(
            string companyName,
            string? registrationNumber,
            string? address,
            string? phone,
            string? email,
            string? LogoUrl,
            string createdBy,
            CancellationToken cancellationToken = default);

        Task<Company> UpdateCompanyProfileAsync(
            int companyId,
            string companyName,
            string? registrationNumber,
            string? address,
            string? phone,
            string? email,
            string? LogoUrl,
            string updatedBy,
            CancellationToken cancellationToken = default);

        Task<Company> ActivateCompanyAsync(
            int companyId,
            string activatedBy,
            CancellationToken cancellationToken = default);

        Task<Company> DeactivateCompanyAsync(
            int companyId,
            string deactivatedBy,
            CancellationToken cancellationToken = default);

        Task ValidateCompanyAsync(
            Company company,
            CancellationToken cancellationToken = default);

        Task<Company> GetValidatedCompanyByIdAsync(
            int companyId,
            CancellationToken cancellationToken = default);

        // âœ… ============================================================
        // âœ… DUPLICATE CHECK METHODS - ADDED
        // âœ… ============================================================

        /// <summary>
        /// Check if company name is unique
        /// </summary>
        Task<bool> IsCompanyNameUniqueAsync(
            string companyName,
            int? excludeCompanyId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if registration number is unique
        /// </summary>
        Task<bool> IsRegistrationNumberUniqueAsync(
            string registrationNumber,
            int? excludeCompanyId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if email is unique
        /// </summary>
        Task<bool> IsEmailUniqueAsync(
            string email,
            int? excludeCompanyId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if phone is unique
        /// </summary>
        Task<bool> IsPhoneUniqueAsync(
            string phone,
            int? excludeCompanyId = null,
            CancellationToken cancellationToken = default);
    }
}
