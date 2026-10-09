using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class EmployeeBankAccountRepository : IEmployeeBankAccountRepository
    {
        private readonly AppDbContext _context;

        public EmployeeBankAccountRepository(AppDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET BY ID
        // =========================================================

        public async Task<EmployeeBankAccount?> GetByIdAsync(int id)
        {
            return await _context.EmployeeBankAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(ba => ba.BankAccountId == id);
        }

        // =========================================================
        // GET BY ID WITH DETAILS
        // =========================================================

        public async Task<EmployeeBankAccount?> GetByIdWithDetailsAsync(int id)
        {
            return await _context.EmployeeBankAccounts
                .AsNoTracking()
                .Include(ba => ba.Employee)
                    .ThenInclude(e => e.Department)
                .Include(ba => ba.Employee)
                    .ThenInclude(e => e.Designation)
                .Include(ba => ba.Employee)
                    .ThenInclude(e => e.Branch)
                .FirstOrDefaultAsync(ba => ba.BankAccountId == id);
        }

        // =========================================================
        // GET BY EMPLOYEE
        // =========================================================

        public async Task<IEnumerable<EmployeeBankAccount>> GetByEmployeeIdAsync(
            int employeeId)
        {
            return await _context.EmployeeBankAccounts
                .AsNoTracking()
                .Where(ba => ba.EmployeeId == employeeId)
                .Include(ba => ba.Employee)
                .OrderByDescending(ba => ba.IsPrimary)
                .ThenBy(ba => ba.BankName)
                .ToListAsync();
        }

        // =========================================================
        // GET ACTIVE BY EMPLOYEE
        // =========================================================

        public async Task<IEnumerable<EmployeeBankAccount>> GetActiveByEmployeeIdAsync(
            int employeeId)
        {
            return await _context.EmployeeBankAccounts
                .AsNoTracking()
                .Where(ba =>
                    ba.EmployeeId == employeeId &&
                    ba.IsActive == true)
                .Include(ba => ba.Employee)
                .OrderByDescending(ba => ba.IsPrimary)
                .ThenBy(ba => ba.BankName)
                .ToListAsync();
        }

        // =========================================================
        // GET PRIMARY ACCOUNT
        // =========================================================

        public async Task<EmployeeBankAccount?> GetPrimaryByEmployeeIdAsync(
            int employeeId)
        {
            return await _context.EmployeeBankAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(ba =>
                    ba.EmployeeId == employeeId &&
                    ba.IsPrimary == true &&
                    ba.IsActive == true);
        }

        // =========================================================
        // PAGED BY EMPLOYEE
        // =========================================================

        public async Task<IEnumerable<EmployeeBankAccount>> GetPagedByEmployeeIdAsync(
            int employeeId,
            int page,
            int pageSize)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            return await _context.EmployeeBankAccounts
                .AsNoTracking()
                .Where(ba => ba.EmployeeId == employeeId)
                .Include(ba => ba.Employee)
                .OrderByDescending(ba => ba.IsPrimary)
                .ThenBy(ba => ba.BankName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        // =========================================================
        // COUNT BY EMPLOYEE
        // =========================================================

        public async Task<int> GetCountByEmployeeIdAsync(int employeeId)
        {
            return await _context.EmployeeBankAccounts
                .CountAsync(ba => ba.EmployeeId == employeeId);
        }

        // =========================================================
        // ACCOUNT EXISTS
        // =========================================================

        public async Task<bool> AccountExistsAsync(
            string accountNumber,
            int? excludeId = null)
        {
            var query = _context.EmployeeBankAccounts
                .Where(ba => ba.AccountNumber == accountNumber);

            if (excludeId.HasValue)
            {
                query = query.Where(ba =>
                    ba.BankAccountId != excludeId);
            }

            return await query.AnyAsync();
        }

        // =========================================================
        // ACCOUNT EXISTS FOR EMPLOYEE
        // =========================================================

        public async Task<bool> AccountExistsForEmployeeAsync(
            int employeeId,
            string accountNumber,
            int? excludeId = null)
        {
            var query = _context.EmployeeBankAccounts
                .Where(ba =>
                    ba.EmployeeId == employeeId &&
                    ba.AccountNumber == accountNumber);

            if (excludeId.HasValue)
            {
                query = query.Where(ba =>
                    ba.BankAccountId != excludeId);
            }

            return await query.AnyAsync();
        }

        // =========================================================
        // PRIMARY ACCOUNT EXISTS
        // =========================================================

        public async Task<bool> EmployeeHasPrimaryAccountAsync(
            int employeeId,
            int? excludeId = null)
        {
            var query = _context.EmployeeBankAccounts
                .Where(ba =>
                    ba.EmployeeId == employeeId &&
                    ba.IsPrimary == true &&
                    ba.IsActive == true);

            if (excludeId.HasValue)
            {
                query = query.Where(ba =>
                    ba.BankAccountId != excludeId);
            }

            return await query.AnyAsync();
        }

        // =========================================================
        // TOTAL COUNT
        // =========================================================

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.EmployeeBankAccounts
                .CountAsync();
        }

        // =========================================================
        // FILTER + SEARCH + DATE RANGE + PAGINATION
        // =========================================================

        public async Task<(List<EmployeeBankAccount> Items, int TotalCount)>
            SearchAsync(
                int? employeeId,
                string? searchTerm,
                string? bankName,
                bool? isActive,
                bool? isPrimary,
                DateTime? dateFrom,
                DateTime? dateTo,
                string? sortBy,
                bool sortDesc,
                int page,
                int pageSize)
        {
            if (page < 1)
                page = 1;

            if (pageSize < 1)
                pageSize = 10;

            var query = _context.EmployeeBankAccounts
                .AsNoTracking()
                .Include(ba => ba.Employee)
                    .ThenInclude(e => e.Department)
                .Include(ba => ba.Employee)
                    .ThenInclude(e => e.Designation)
                .Include(ba => ba.Employee)
                    .ThenInclude(e => e.Branch)
                .AsQueryable();

            // =====================================================
            // EMPLOYEE FILTER
            // =====================================================

            if (employeeId.HasValue)
            {
                query = query.Where(ba =>
                    ba.EmployeeId == employeeId);
            }

            // =====================================================
            // GENERAL SEARCH
            // Employee Code
            // First Name
            // Last Name
            // Account Number
            // IBAN
            // Bank Name
            // =====================================================

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim();

                query = query.Where(ba =>
                    (ba.AccountNumber != null &&
                     ba.AccountNumber.ToLower().Contains(term.ToLower())) ||

                    (ba.BankName != null &&
                     ba.BankName.ToLower().Contains(term.ToLower())) ||

                    (ba.Iban != null &&
                     ba.Iban.ToLower().Contains(term.ToLower())) ||

                    (ba.Employee.EmployeeCode != null &&
                     ba.Employee.EmployeeCode
                         .ToLower()
                         .Contains(term.ToLower())) ||

                    (ba.Employee.FirstName != null &&
                     ba.Employee.FirstName
                         .ToLower()
                         .Contains(term.ToLower())) ||

                    (ba.Employee.LastName != null &&
                     ba.Employee.LastName
                         .ToLower()
                         .Contains(term.ToLower()))
                );
            }

            // =====================================================
            // BANK NAME FILTER
            // =====================================================

            if (!string.IsNullOrWhiteSpace(bankName))
            {
                var bank = bankName.Trim();

                query = query.Where(ba =>
                    ba.BankName.ToLower().Contains(bank.ToLower()));
            }

            // =====================================================
            // ACTIVE / INACTIVE FILTER
            // =====================================================

            if (isActive.HasValue)
            {
                query = query.Where(ba =>
                    ba.IsActive == isActive);
            }

            // =====================================================
            // PRIMARY / NON PRIMARY FILTER
            // =====================================================

            if (isPrimary.HasValue)
            {
                query = query.Where(ba =>
                    ba.IsPrimary == isPrimary);
            }

            // =====================================================
            // DATE FROM
            // =====================================================

            if (dateFrom.HasValue)
            {
                query = query.Where(ba =>
                    ba.CreatedDate >= dateFrom);
            }

            // =====================================================
            // DATE TO
            // =====================================================

            if (dateTo.HasValue)
            {
                var endDate = dateTo.Value.Date.AddDays(1);

                query = query.Where(ba =>
                    ba.CreatedDate < endDate);
            }

            // =====================================================
            // TOTAL COUNT BEFORE PAGINATION
            // =====================================================

            var totalCount = await query.CountAsync();

            // =====================================================
            // SORTING
            // =====================================================

            query = sortBy?.Trim().ToLower() switch
            {
                "bankname" => sortDesc
                    ? query.OrderByDescending(ba => ba.BankName)
                    : query.OrderBy(ba => ba.BankName),

                "accountnumber" => sortDesc
                    ? query.OrderByDescending(ba => ba.AccountNumber)
                    : query.OrderBy(ba => ba.AccountNumber),

                "createddate" => sortDesc
                    ? query.OrderByDescending(ba => ba.CreatedDate)
                    : query.OrderBy(ba => ba.CreatedDate),

                "employee" => sortDesc
                    ? query.OrderByDescending(ba =>
                        ba.Employee.FirstName)
                    : query.OrderBy(ba =>
                        ba.Employee.FirstName),

                _ => sortDesc
                    ? query.OrderByDescending(ba => ba.BankAccountId)
                    : query.OrderBy(ba => ba.BankAccountId)
            };

            // =====================================================
            // PAGINATION
            // =====================================================

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // =========================================================
        // GET REPORT WITHOUT PAGINATION
        // =========================================================

        public async Task<List<EmployeeBankAccount>> GetReportAsync(
            int? employeeId,
            string? searchTerm,
            string? bankName,
            bool? isActive,
            bool? isPrimary,
            DateTime? dateFrom,
            DateTime? dateTo)
        {
            var query = _context.EmployeeBankAccounts
                .AsNoTracking()
                .Include(ba => ba.Employee)
                    .ThenInclude(e => e.Department)
                .Include(ba => ba.Employee)
                    .ThenInclude(e => e.Designation)
                .Include(ba => ba.Employee)
                    .ThenInclude(e => e.Branch)
                .AsQueryable();

            // Employee
            if (employeeId.HasValue)
            {
                query = query.Where(ba =>
                    ba.EmployeeId == employeeId);
            }

            // Search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();

                query = query.Where(ba =>
                    ba.AccountNumber.ToLower().Contains(term) ||
                    ba.BankName.ToLower().Contains(term) ||
                    (ba.Iban != null &&
                     ba.Iban.ToLower().Contains(term)) ||
                    ba.Employee.EmployeeCode.ToLower().Contains(term) ||
                    ba.Employee.FirstName.ToLower().Contains(term) ||
                    ba.Employee.LastName.ToLower().Contains(term));
            }

            // Bank
            if (!string.IsNullOrWhiteSpace(bankName))
            {
                var bank = bankName.Trim().ToLower();

                query = query.Where(ba =>
                    ba.BankName.ToLower().Contains(bank));
            }

            // Active
            if (isActive.HasValue)
            {
                query = query.Where(ba =>
                    ba.IsActive == isActive);
            }

            // Primary
            if (isPrimary.HasValue)
            {
                query = query.Where(ba =>
                    ba.IsPrimary == isPrimary);
            }

            // From date
            if (dateFrom.HasValue)
            {
                query = query.Where(ba =>
                    ba.CreatedDate >= dateFrom);
            }

            // To date
            if (dateTo.HasValue)
            {
                var endDate = dateTo.Value.Date.AddDays(1);

                query = query.Where(ba =>
                    ba.CreatedDate < endDate);
            }

            return await query
                .OrderBy(ba => ba.Employee.FirstName)
                .ThenBy(ba => ba.Employee.LastName)
                .ThenBy(ba => ba.BankName)
                .ToListAsync();
        }

        // =========================================================
        // CREATE
        // =========================================================

        public async Task AddAsync(EmployeeBankAccount account)
        {
            await _context.EmployeeBankAccounts.AddAsync(account);
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public void Update(EmployeeBankAccount account)
        {
            _context.EmployeeBankAccounts.Update(account);
        }

        // =========================================================
        // SAVE
        // =========================================================

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
