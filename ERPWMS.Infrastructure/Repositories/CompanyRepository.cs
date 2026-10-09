using ERPWMS.Domain.Interfaces;
using ERPWMS.Infrastructure.Data;
using ERPWMS.Database.Models;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Company> _dbSet;

        public CompanyRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Company>();
        }

        // ============================================================
        // COMPANY RETRIEVAL
        // ============================================================

        public async Task<Company?> GetCompanyByIdAsync(
            int companyId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .FirstOrDefaultAsync(
                    c => c.CompanyId == companyId,
                    cancellationToken);
        }

        public async Task<Company?> GetCompanyByNameAsync(
            string companyName,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                return null;
            }

            companyName = companyName.Trim();

            return await _dbSet
                .FirstOrDefaultAsync(
                    c => c.CompanyName == companyName,
                    cancellationToken);
        }

        public async Task<Company?> GetCompanyByRegistrationNumberAsync(
            string registrationNumber,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
            {
                return null;
            }

            registrationNumber = registrationNumber.Trim();

            return await _dbSet
                .FirstOrDefaultAsync(
                    c => c.RegistrationNo == registrationNumber,
                    cancellationToken);
        }

        public async Task<IEnumerable<Company>> GetAllCompaniesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .OrderBy(c => c.CompanyName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Company>> GetActiveCompaniesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .AsNoTracking()
                .Where(c => c.IsActive == true)
                .OrderBy(c => c.CompanyName)
                .ToListAsync(cancellationToken);
        }

        // ============================================================
        // COMPANY SEARCH
        // ============================================================

        public async Task<PagedResult<Company>> SearchCompaniesAsync(
            string? searchTerm,
            bool? isActive,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDescending,
            CancellationToken cancellationToken = default)
        {
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1)
            {
                pageSize = 10;
            }

            var query = _dbSet.AsNoTracking().AsQueryable();

            // --------------------------------------------------------
            // SEARCH
            // --------------------------------------------------------

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(c =>
                    c.CompanyName.Contains(searchTerm) ||

                    (c.RegistrationNo != null &&
                     c.RegistrationNo.Contains(searchTerm)) ||

                    (c.Email != null &&
                     c.Email.Contains(searchTerm)) ||

                    (c.Phone != null &&
                     c.Phone.Contains(searchTerm)) ||

                    (c.Address != null &&
                     c.Address.Contains(searchTerm)));
            }

            // --------------------------------------------------------
            // ACTIVE FILTER
            // --------------------------------------------------------

            if (isActive.HasValue)
            {
                query = query.Where(
                    c => c.IsActive == isActive);
            }

            // --------------------------------------------------------
            // TOTAL COUNT
            // --------------------------------------------------------

            var totalCount =
                await query.CountAsync(cancellationToken);

            // --------------------------------------------------------
            // SORT
            // --------------------------------------------------------

            query = ApplySorting(
                query,
                sortBy,
                sortDescending);

            // --------------------------------------------------------
            // PAGINATION
            // --------------------------------------------------------

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Company>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        // ============================================================
        // COMPANY DASHBOARD
        // ============================================================

        public async Task<int> GetTotalCompanyCountAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .CountAsync(cancellationToken);
        }

        public async Task<int> GetActiveCompanyCountAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .CountAsync(
                    c => c.IsActive == true,
                    cancellationToken);
        }

        public async Task<int> GetInactiveCompanyCountAsync(
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .CountAsync(
                    c => c.IsActive != true,
                    cancellationToken);
        }

        public async Task<int> GetCompaniesAddedThisMonthAsync(
            CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            var startOfMonth = new DateTime(
                now.Year,
                now.Month,
                1);

            var startOfNextMonth =
                startOfMonth.AddMonths(1);

            return await _dbSet.CountAsync(
                c =>
                    c.CreatedDate >= startOfMonth &&
                    c.CreatedDate < startOfNextMonth,
                cancellationToken);
        }

        public async Task<IEnumerable<Company>> GetRecentCompaniesAsync(
            int count,
            CancellationToken cancellationToken = default)
        {
            if (count <= 0)
            {
                return new List<Company>();
            }

            return await _dbSet
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedDate)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        // ============================================================
        // WAREHOUSE COUNT
        // ============================================================
        //
        // IMPORTANT:
        // Warehouse entity mein CompanyId nahi hai.
        // Isliye CompanyId ke through filter nahi karna.
        //
        // Ye database ki Warehouse table se total records count karega.
        // ============================================================

        public async Task<int> GetTotalWarehouseCountAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context
                .Set<Warehouse>()
                .CountAsync(cancellationToken);
        }

        // ============================================================
        // BRANCH COUNT
        // ============================================================
        //
        // Ye database ki Branch table se total records count karega.
        // ============================================================

        public async Task<int> GetTotalBranchCountAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context
                .Set<Branch>()
                .CountAsync(cancellationToken);
        }

        // ============================================================
        // SORTING
        // ============================================================

        private static IQueryable<Company> ApplySorting(
            IQueryable<Company> query,
            string? sortBy,
            bool sortDescending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderBy(c => c.CompanyName);
            }

            return (sortBy.Trim().ToLowerInvariant(), sortDescending)
                switch
            {
                ("companyname", false) =>
                    query.OrderBy(c => c.CompanyName),

                ("companyname", true) =>
                    query.OrderByDescending(c => c.CompanyName),

                ("registrationnumber", false) =>
                    query.OrderBy(
                        c => c.RegistrationNo ?? string.Empty),

                ("registrationnumber", true) =>
                    query.OrderByDescending(
                        c => c.RegistrationNo ?? string.Empty),

                ("createddate", false) =>
                    query.OrderBy(c => c.CreatedDate),

                ("createddate", true) =>
                    query.OrderByDescending(c => c.CreatedDate),

                ("isactive", false) =>
                    query.OrderBy(c => c.IsActive),

                ("isactive", true) =>
                    query.OrderByDescending(c => c.IsActive),

                _ =>
                    query.OrderBy(c => c.CompanyName)
            };
        }

        // ============================================================
        // COMPANY NAME VALIDATION
        // ============================================================

        public async Task<bool> IsCompanyNameUniqueAsync(
            string companyName,
            int? excludingCompanyId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(companyName))
            {
                return false;
            }

            companyName = companyName.Trim();

            var query = _dbSet.Where(
                c => c.CompanyName == companyName);

            if (excludingCompanyId.HasValue)
            {
                query = query.Where(
                    c => c.CompanyId != excludingCompanyId);
            }

            return !await query.AnyAsync(
                cancellationToken);
        }

        // ============================================================
        // REGISTRATION NUMBER VALIDATION
        // ============================================================

        public async Task<bool> IsRegistrationNumberUniqueAsync(
            string registrationNumber,
            int? excludingCompanyId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(registrationNumber))
            {
                return true;
            }

            registrationNumber = registrationNumber.Trim();

            var query = _dbSet.Where(
                c => c.RegistrationNo == registrationNumber);

            if (excludingCompanyId.HasValue)
            {
                query = query.Where(
                    c => c.CompanyId != excludingCompanyId);
            }

            return !await query.AnyAsync(
                cancellationToken);
        }

        // ============================================================
        // EMAIL VALIDATION
        // ============================================================

        public async Task<bool> IsEmailUniqueAsync(
            string email,
            int? excludingCompanyId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return true;
            }

            email = email.Trim();

            var query = _dbSet.Where(
                c =>
                    c.Email != null &&
                    c.Email == email);

            if (excludingCompanyId.HasValue)
            {
                query = query.Where(
                    c => c.CompanyId != excludingCompanyId);
            }

            return !await query.AnyAsync(
                cancellationToken);
        }

        // ============================================================
        // PHONE VALIDATION
        // ============================================================

        public async Task<bool> IsPhoneUniqueAsync(
            string phone,
            int? excludingCompanyId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(phone))
            {
                return true;
            }

            phone = phone.Trim();

            var query = _dbSet.Where(
                c =>
                    c.Phone != null &&
                    c.Phone == phone);

            if (excludingCompanyId.HasValue)
            {
                query = query.Where(
                    c => c.CompanyId != excludingCompanyId);
            }

            return !await query.AnyAsync(
                cancellationToken);
        }

        // ============================================================
        // DEPENDENCIES
        // ============================================================

        public async Task<bool> HasActiveDependenciesAsync(
            int companyId,
            CancellationToken cancellationToken = default)
        {
            // Abhi Company ke active dependencies ka direct
            // relationship defined nahi hai.
            //
            // Isliye false return kiya ja raha hai.
            //
            // Future mein Branch/Warehouse/etc. ke actual FK
            // relation ke according yahan validation add ki ja sakti hai.

            await Task.CompletedTask;

            return false;
        }

        // ============================================================
        // COMMAND - ADD
        // ============================================================

        public async Task AddCompanyAsync(
            Company company,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(company);

            await _dbSet.AddAsync(
                company,
                cancellationToken);
        }

        // ============================================================
        // COMMAND - UPDATE
        // ============================================================

        public void UpdateCompany(Company company)
        {
            ArgumentNullException.ThrowIfNull(company);

            _dbSet.Update(company);
        }

        // ============================================================
        // COMMAND - REMOVE
        // ============================================================

        public void RemoveCompany(Company company)
        {
            ArgumentNullException.ThrowIfNull(company);

            _dbSet.Remove(company);
        }

        // ============================================================
        // SAVE
        // ============================================================

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(
                cancellationToken);
        }
    }
}
