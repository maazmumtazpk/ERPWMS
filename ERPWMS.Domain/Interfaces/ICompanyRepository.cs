using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database.Models;

namespace ERPWMS.Domain.Interfaces
{
    public interface ICompanyRepository
    {
        // ============================================================
        // COMPANY RETRIEVAL
        // ============================================================

        Task<Company?> GetCompanyByIdAsync(
            int companyId,
            CancellationToken cancellationToken = default);

        Task<Company?> GetCompanyByNameAsync(
            string companyName,
            CancellationToken cancellationToken = default);

        Task<Company?> GetCompanyByRegistrationNumberAsync(
            string registrationNumber,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Company>> GetAllCompaniesAsync(
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Company>> GetActiveCompaniesAsync(
            CancellationToken cancellationToken = default);


        // ============================================================
        // COMPANY SEARCH
        // ============================================================

        Task<PagedResult<Company>> SearchCompaniesAsync(
            string? searchTerm,
            bool? isActive,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDescending,
            CancellationToken cancellationToken = default);


        // ============================================================
        // COMPANY DASHBOARD
        // ============================================================

        Task<int> GetTotalCompanyCountAsync(
            CancellationToken cancellationToken = default);

        Task<int> GetActiveCompanyCountAsync(
            CancellationToken cancellationToken = default);

        Task<int> GetInactiveCompanyCountAsync(
            CancellationToken cancellationToken = default);

        Task<int> GetCompaniesAddedThisMonthAsync(
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Company>> GetRecentCompaniesAsync(
            int count,
            CancellationToken cancellationToken = default);


        // ============================================================
        // BRANCH & WAREHOUSE DASHBOARD
        // ============================================================

        /// <summary>
        /// Gets total number of branches directly from Branch table.
        /// </summary>
        Task<int> GetTotalBranchCountAsync(
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets total number of warehouses directly from Warehouse table.
        /// </summary>
        Task<int> GetTotalWarehouseCountAsync(
            CancellationToken cancellationToken = default);


        // ============================================================
        // VALIDATION
        // ============================================================

        Task<bool> IsCompanyNameUniqueAsync(
            string companyName,
            int? excludingCompanyId = null,
            CancellationToken cancellationToken = default);

        Task<bool> IsRegistrationNumberUniqueAsync(
            string registrationNumber,
            int? excludingCompanyId = null,
            CancellationToken cancellationToken = default);

        Task<bool> HasActiveDependenciesAsync(
            int companyId,
            CancellationToken cancellationToken = default);


        // ============================================================
        // EMAIL VALIDATION
        // ============================================================

        Task<bool> IsEmailUniqueAsync(
            string email,
            int? excludingCompanyId = null,
            CancellationToken cancellationToken = default);


        // ============================================================
        // PHONE VALIDATION
        // ============================================================

        Task<bool> IsPhoneUniqueAsync(
            string phone,
            int? excludingCompanyId = null,
            CancellationToken cancellationToken = default);


        // ============================================================
        // COMMAND OPERATIONS
        // ============================================================

        Task AddCompanyAsync(
            Company company,
            CancellationToken cancellationToken = default);

        void UpdateCompany(Company company);

        void RemoveCompany(Company company);

        Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default);
    }


    // ============================================================
    // PAGED RESULT
    // ============================================================

    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; }
            = new List<T>();

        public int TotalCount { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalPages =>
            PageSize > 0
                ? (int)Math.Ceiling(
                    (double)TotalCount / PageSize)
                : 0;
    }
}
