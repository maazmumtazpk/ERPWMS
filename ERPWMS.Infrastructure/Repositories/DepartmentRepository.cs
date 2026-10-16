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
    /// Repository implementation for Department aggregate root
    /// </summary>
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Department> _dbSet;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Department>();
        }

        public async Task<Department?> GetDepartmentByIdAsync(
            int departmentId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Branch)
                .Include(d => d.Company)
                .FirstOrDefaultAsync(
                    d => d.DepartmentId == departmentId,
                    cancellationToken);
        }

        public async Task<Department?> GetDepartmentByNameAsync(
            string departmentName,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Branch)
                .Include(d => d.Company)
                .FirstOrDefaultAsync(
                    d => d.DepartmentName == departmentName &&
                         d.BranchId == branchId,
                    cancellationToken);
        }

        public async Task<Department?> GetDepartmentByCodeAsync(
            string departmentCode,
            int branchId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(departmentCode))
                return null;

            return await _dbSet
                .Include(d => d.Branch)
                .Include(d => d.Company)
                .FirstOrDefaultAsync(
                    d => d.DepartmentCode == departmentCode &&
                         d.BranchId == branchId,
                    cancellationToken);
        }

        public async Task<IEnumerable<Department>> GetDepartmentsByBranchAsync(
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d => d.BranchId == branchId)
                .Include(d => d.Branch)
                .Include(d => d.Company)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Department>> GetActiveDepartmentsByBranchAsync(
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d => d.BranchId == branchId &&
                            d.IsActive == true)
                .Include(d => d.Branch)
                .Include(d => d.Company)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Department>> GetDepartmentsByCompanyAsync(
            int companyId,
            CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d => d.CompanyId == companyId)
                .Include(d => d.Branch)
                .Include(d => d.Company)
                .OrderBy(d => d.DepartmentName)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Department>> SearchDepartmentsAsync(
            string? searchTerm,
            int? branchId,
            int? companyId,
            bool? isActive,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDescending,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();

            // Apply branch filter
            if (branchId.HasValue)
            {
                query = query.Where(d => d.BranchId == branchId);
            }

            // Apply company filter
            if (companyId.HasValue)
            {
                query = query.Where(d => d.CompanyId == companyId);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                query = query.Where(d =>
                    d.DepartmentName.Contains(searchTerm) ||
                    (d.DepartmentCode != null &&
                     d.DepartmentCode.Contains(searchTerm)));
            }

            // Apply status filter
            if (isActive.HasValue)
            {
                query = query.Where(d => d.IsActive == isActive);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply sorting
            query = ApplySorting(query, sortBy, sortDescending);

            // Apply pagination
            var items = await query
                .Include(d => d.Branch)
                .Include(d => d.Company)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Department>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        private IQueryable<Department> ApplySorting(
            IQueryable<Department> query,
            string? sortBy,
            bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return query.OrderBy(d => d.DepartmentName);

            return (sortBy.ToLower(), sortDescending) switch
            {
                ("departmentname", false) =>
                    query.OrderBy(d => d.DepartmentName),

                ("departmentname", true) =>
                    query.OrderByDescending(d => d.DepartmentName),

                ("departmentcode", false) =>
                    query.OrderBy(d => d.DepartmentCode ?? string.Empty),

                ("departmentcode", true) =>
                    query.OrderByDescending(d => d.DepartmentCode ?? string.Empty),

                ("createddate", false) =>
                    query.OrderBy(d => d.CreatedDate),

                ("createddate", true) =>
                    query.OrderByDescending(d => d.CreatedDate),

                ("isactive", false) =>
                    query.OrderBy(d => d.IsActive),

                ("isactive", true) =>
                    query.OrderByDescending(d => d.IsActive),

                _ =>
                    query.OrderBy(d => d.DepartmentName)
            };
        }

        public async Task<bool> IsDepartmentNameUniqueAsync(
            string departmentName,
            int branchId,
            int? excludingDepartmentId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(d =>
                d.DepartmentName == departmentName &&
                d.BranchId == branchId);

            if (excludingDepartmentId.HasValue)
            {
                query = query.Where(d =>
                    d.DepartmentId != excludingDepartmentId);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> IsDepartmentCodeUniqueAsync(
            string departmentCode,
            int branchId,
            int? excludingDepartmentId = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(departmentCode))
                return true;

            var query = _dbSet.Where(d =>
                d.DepartmentCode == departmentCode &&
                d.BranchId == branchId);

            if (excludingDepartmentId.HasValue)
            {
                query = query.Where(d =>
                    d.DepartmentId != excludingDepartmentId);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> HasActiveDependenciesAsync(
            int departmentId,
            CancellationToken cancellationToken = default)
        {
            // Check for active designations
            // This will be implemented when Designation module exists

            // Check for active employees
            // This will be implemented when Employee module exists

            // For now, assume no dependencies
            return await Task.FromResult(false);
        }

        public async Task<bool> BranchExistsAsync(
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Branches
                .AnyAsync(
                    b => b.BranchId == branchId &&
                         b.IsDeleted == false,
                    cancellationToken);
        }

        public async Task<bool> IsBranchActiveAsync(
            int branchId,
            CancellationToken cancellationToken = default)
        {
            var branch = await _context.Branches
                .FirstOrDefaultAsync(
                    b => b.BranchId == branchId &&
                         b.IsDeleted == false,
                    cancellationToken);

            return branch != null && branch.IsActive == true;
        }

        public async Task<bool> CompanyExistsAsync(
            int companyId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                .AnyAsync(
                    c => c.CompanyId == companyId &&
                         c.IsDeleted == false,
                    cancellationToken);
        }

        // ============================================================
        // NEW METHOD
        // Gets CompanyId automatically from the selected active Branch
        // BranchId -> Branch.CompanyId
        // ============================================================
        public async Task<int?> GetCompanyIdByBranchAsync(
            int branchId,
            CancellationToken cancellationToken = default)
        {
            return await _context.Branches
                .Where(b =>
                    b.BranchId == branchId &&
                    b.IsDeleted == false &&
                    b.IsActive == true)
                .Select(b => (int?)b.CompanyId)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task AddDepartmentAsync(
            Department department,
            CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(department, cancellationToken);
        }

        public void UpdateDepartment(Department department)
        {
            _dbSet.Update(department);
        }

        public void RemoveDepartment(Department department)
        {
            _dbSet.Remove(department);
        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}

