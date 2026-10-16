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
    public class DesignationRepository : IDesignationRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Designation> _dbSet;

        public DesignationRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Designation>();
        }

        public async Task<Designation?> GetDesignationByIdAsync(int designationId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Department)
                .ThenInclude(dept => dept.Company)
                .FirstOrDefaultAsync(d => d.DesignationId == designationId, cancellationToken);
        }

        public async Task<Designation?> GetDesignationByNameAsync(string designationName, int departmentId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(d => d.Department)
                .Include(d => d.Department.Company)
                .FirstOrDefaultAsync(d => d.DesignationName == designationName && d.DepartmentId == departmentId, cancellationToken);
        }

        public async Task<IEnumerable<Designation>> GetDesignationsByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d => d.DepartmentId == departmentId)
                .Include(d => d.Department)
                .Include(d => d.Department.Company)
                .OrderBy(d => d.DesignationName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Designation>> GetActiveDesignationsByDepartmentAsync(int departmentId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d => d.DepartmentId == departmentId && d.IsActive == true)
                .Include(d => d.Department)
                .Include(d => d.Department.Company)
                .OrderBy(d => d.DesignationName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Designation>> GetDesignationsByCompanyAsync(int companyId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(d => d.Department.CompanyId == companyId)
                .Include(d => d.Department)
                .Include(d => d.Department.Company)
                .OrderBy(d => d.DesignationName)
                .ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<Designation>> SearchDesignationsAsync(
            string? searchTerm,
            int? departmentId,
            int? companyId,
            bool? isActive,
            int page,
            int pageSize,
            string? sortBy,
            bool sortDescending,
            CancellationToken cancellationToken = default)
        {
            var query = _dbSet.AsQueryable();

            // Apply department filter
            if (departmentId.HasValue)
            {
                query = query.Where(d => d.DepartmentId == departmentId);
            }

            // Apply company filter
            if (companyId.HasValue)
            {
                query = query.Where(d => d.Department.CompanyId == companyId);
            }

            // Apply search filter
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();
                query = query.Where(d =>
                    d.DesignationName.Contains(searchTerm) ||
                    (d.RankLevel.HasValue && d.RankLevel.Value.ToString().Contains(searchTerm)));
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
                .Include(d => d.Department)
                .Include(d => d.Department.Company)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<Designation>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        private IQueryable<Designation> ApplySorting(IQueryable<Designation> query, string? sortBy, bool sortDescending)
        {
            if (string.IsNullOrEmpty(sortBy))
                return query.OrderBy(d => d.DesignationName);

            return (sortBy.ToLower(), sortDescending) switch
            {
                ("designationname", false) => query.OrderBy(d => d.DesignationName),
                ("designationname", true) => query.OrderByDescending(d => d.DesignationName),
                ("ranklevel", false) => query.OrderBy(d => d.RankLevel ?? 0),
                ("ranklevel", true) => query.OrderByDescending(d => d.RankLevel ?? 0),
                ("createddate", false) => query.OrderBy(d => d.CreatedDate),
                ("createddate", true) => query.OrderByDescending(d => d.CreatedDate),
                ("isactive", false) => query.OrderBy(d => d.IsActive),
                ("isactive", true) => query.OrderByDescending(d => d.IsActive),
                _ => query.OrderBy(d => d.DesignationName)
            };
        }

        public async Task<bool> IsDesignationNameUniqueAsync(string designationName, int departmentId, int? excludingDesignationId = null, CancellationToken cancellationToken = default)
        {
            var query = _dbSet.Where(d => d.DesignationName == designationName && d.DepartmentId == departmentId);

            if (excludingDesignationId.HasValue)
            {
                query = query.Where(d => d.DesignationId != excludingDesignationId);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        public async Task<bool> HasActiveDependenciesAsync(int designationId, CancellationToken cancellationToken = default)
        {
            // Check for active employees
            // This will be implemented when Employee module exists
            // var hasActiveEmployees = await _context.Employees
            //     .AnyAsync(e => e.DesignationId == designationId && e.IsActive == true, cancellationToken);

            return await Task.FromResult(false);
        }

        public async Task<bool> DepartmentExistsAsync(int departmentId, CancellationToken cancellationToken = default)
        {
            return await _context.Departments
                .AnyAsync(d => d.DepartmentId == departmentId && d.IsDeleted == false, cancellationToken);
        }

        public async Task<bool> IsDepartmentActiveAsync(int departmentId, CancellationToken cancellationToken = default)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == departmentId && d.IsDeleted == false, cancellationToken);

            return department != null && department.IsActive == true;
        }

        public async Task<bool> CompanyExistsAsync(int companyId, CancellationToken cancellationToken = default)
        {
            return await _context.Companies
                .AnyAsync(c => c.CompanyId == companyId && c.IsDeleted == false, cancellationToken);
        }

        public async Task AddDesignationAsync(Designation designation, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(designation, cancellationToken);
        }

        public void UpdateDesignation(Designation designation)
        {
            _dbSet.Update(designation);
        }

        public void RemoveDesignation(Designation designation)
        {
            _dbSet.Remove(designation);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}



