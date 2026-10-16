using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Database.Models;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Infrastructure.Data;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for Branch aggregate root
    /// </summary>
    public class BranchRepository : IBranchRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Branch> _dbSet;

        public BranchRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Branch>();
        }

        public async Task<Branch?> GetBranchByIdAsync(int branchId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(b => b.Company)
                .FirstOrDefaultAsync(b => b.BranchId == branchId, cancellationToken);
        }

        public async Task<Branch?> GetBranchByNameAsync(string branchName, int companyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(b => b.Company)
                .FirstOrDefaultAsync(b => b.BranchName == branchName && b.CompanyId == companyId, cancellationToken);
        }

        public async Task<Branch?> GetBranchByCodeAsync(string branchCode, int companyId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(branchCode))
                return null;

            return await _dbSet
                .Include(b => b.Company)
                .FirstOrDefaultAsync(b => b.BranchCode == branchCode && b.CompanyId == companyId, cancellationToken);
        }

        public async Task<IEnumerable<Branch>> GetBranchesByCompanyAsync(int companyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(b => b.CompanyId == companyId)
                .Include(b => b.Company)
                .OrderBy(b => b.BranchName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Branch>> GetActiveBranchesByCompanyAsync(int companyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(b => b.CompanyId == companyId && b.IsActive == true)
                .Include(b => b.Company)
                .OrderBy(b => b.BranchName)
                .ToListAsync(cancellationToken);
        }

        // âœ… NEW METHOD - Add this
        public async Task<IEnumerable<Branch>> GetAllActiveBranchesAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(b => b.IsActive == true && b.IsDeleted == false)
                .Include(b => b.Company)
                .OrderBy(b => b.BranchName)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Branch>> SearchBranchesAsync(
            string? searchTerm,
            int? companyId,
            bool? isActive,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDescending,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();

            // Apply company filter
            if (companyId.HasValue)
            {
                query = query.Where(b => b.CompanyId == companyId);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(b =>
                    b.BranchName.Contains(searchTerm) ||
                    (b.BranchCode != null && b.BranchCode.Contains(searchTerm)) ||
                    (b.Email != null && b.Email.Contains(searchTerm)) ||
                    (b.Phone != null && b.Phone.Contains(searchTerm)) ||
                    (b.Address != null && b.Address.Contains(searchTerm)));
            }

            // Apply status filter
            if (isActive.HasValue)
            {
                query = query.Where(b => b.IsActive == isActive);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = ApplySorting(query, sortBy, sortDescending);

            // Apply pagination
            var items = await query
                .Include(b => b.Company)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Branch>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        private IQueryable<Branch> ApplySorting(IQueryable<Branch> query, string? sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return query.OrderBy(b => b.BranchName);

            return (sortBy.ToLower(), sortDescending) switch
            {
                ("branchname", false) => query.OrderBy(b => b.BranchName),
                ("branchname", true) => query.OrderByDescending(b => b.BranchName),
                ("branchcode", false) => query.OrderBy(b => b.BranchCode ?? string.Empty),
                ("branchcode", true) => query.OrderByDescending(b => b.BranchCode ?? string.Empty),
                ("createddate", false) => query.OrderBy(b => b.CreatedDate),
                ("createddate", true) => query.OrderByDescending(b => b.CreatedDate),
                ("isactive", false) => query.OrderBy(b => b.IsActive),
                ("isactive", true) => query.OrderByDescending(b => b.IsActive),
                _ => query.OrderBy(b => b.BranchName)
            };
        }

        public async Task<bool> IsBranchNameUniqueAsync(string branchName, int companyId, int? excludingBranchId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(b => b.BranchName == branchName && b.CompanyId == companyId);

            if (excludingBranchId.HasValue)
            {
                query = query.Where(b => b.BranchId != excludingBranchId);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> IsBranchCodeUniqueAsync(string branchCode, int companyId, int? excludingBranchId = null, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(branchCode))
                return true;

            var query = _dbSet.Where(b => b.BranchCode == branchCode && b.CompanyId == companyId);

            if (excludingBranchId.HasValue)
            {
                query = query.Where(b => b.BranchId != excludingBranchId);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> HasActiveDependenciesAsync(int branchId, CancellationToken cancellationToken = default)
        {
            // Check for active employees
            // This will be implemented when Employee module exists
            // var hasActiveEmployees = await _context.Employees
            //     .AnyAsync(e => e.BranchId == branchId && e.IsActive == true, cancellationToken);

            // Check for active warehouses
            // This will be implemented when Warehouse module exists
            // var hasActiveWarehouses = await _context.Warehouses
            //     .AnyAsync(w => w.BranchId == branchId && w.IsActive == true, cancellationToken);

            // For now, assume no dependencies
            return await Task.FromResult(false);
        }

        public async Task<bool> CompanyExistsAsync(int companyId, CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                .AnyAsync(c => c.CompanyId == companyId && c.IsDeleted == false, cancellationToken);
        }

        public async Task<bool> IsCompanyActiveAsync(int companyId, CancellationToken cancellationToken = default)
        {
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.CompanyId == companyId && c.IsDeleted == false, cancellationToken);

            return company != null && company.IsActive == true;
        }

        public async Task AddBranchAsync(Branch branch, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(branch, cancellationToken);
        }

        public void UpdateBranch(Branch branch)
        {
            _dbSet.Update(branch);
        }

        public void RemoveBranch(Branch branch)
        {
            _dbSet.Remove(branch);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

