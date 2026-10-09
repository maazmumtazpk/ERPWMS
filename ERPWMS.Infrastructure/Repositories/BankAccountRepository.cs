using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly AppDbContext _context;

        public BankAccountRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // GET ALL
        // ============================================================
        public async Task<List<BankAccount>> GetAllAsync(
            int companyId,
            bool includeDeleted = false)
        {
            IQueryable<BankAccount> query = _context.BankAccounts
                .Where(b => b.CompanyId == companyId)
                .Include(b => b.Company);

            if (!includeDeleted)
            {
                query = query.Where(b => b.IsDeleted != true);
            }

            return await query
                .OrderBy(b => b.BankName)
                .ToListAsync();
        }

        // ============================================================
        // GET BY ID
        // ============================================================
        public async Task<BankAccount?> GetByIdAsync(
            int id,
            int companyId)
        {
            return await _context.BankAccounts
                .Where(b =>
                    b.BankAccountId == id &&
                    b.CompanyId == companyId)
                .Include(b => b.Company)
                .FirstOrDefaultAsync();
        }

        // ============================================================
        // GET BY ACCOUNT NUMBER
        // ============================================================
        public async Task<BankAccount?> GetByAccountNoAsync(
            int companyId,
            string accountNo)
        {
            return await _context.BankAccounts
                .Where(b =>
                    b.CompanyId == companyId &&
                    b.AccountNo == accountNo &&
                    b.IsDeleted != true)
                .FirstOrDefaultAsync();
        }

        // ============================================================
        // ACCOUNT NUMBER EXISTS
        // ============================================================
        public async Task<bool> IsAccountNoExistsAsync(
            int companyId,
            string accountNo,
            int? excludeId = null)
        {
            IQueryable<BankAccount> query = _context.BankAccounts
                .Where(b =>
                    b.CompanyId == companyId &&
                    b.AccountNo == accountNo &&
                    b.IsDeleted != true);

            if (excludeId.HasValue)
            {
                query = query.Where(b =>
                    b.BankAccountId != excludeId);
            }

            return await query.AnyAsync();
        }

        // ============================================================
        // IBAN EXISTS
        // ============================================================
        public async Task<bool> IsIbanExistsAsync(
            int companyId,
            string iban,
            int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(iban))
                return false;

            IQueryable<BankAccount> query = _context.BankAccounts
                .Where(b =>
                    b.CompanyId == companyId &&
                    b.Iban == iban &&
                    b.IsDeleted != true);

            if (excludeId.HasValue)
            {
                query = query.Where(b =>
                    b.BankAccountId != excludeId);
            }

            return await query.AnyAsync();
        }

        // ============================================================
        // CAN DELETE
        // ============================================================
        public async Task<bool> CanDeleteAsync(
            int accountId,
            int companyId)
        {
            var account = await GetByIdAsync(accountId, companyId);

            if (account == null || account.IsDeleted == true)
                return false;

            // Check bank transactions
            var hasTransactions = await _context.BankTransactions
                .AnyAsync(t => t.BankAccountId == accountId);

            if (hasTransactions)
                return false;

            // Check reconciliations
            var hasReconciliations = await _context.BankReconciliations
                .AnyAsync(r => r.BankAccountId == accountId);

            if (hasReconciliations)
                return false;

            // Check payment vouchers
            var hasPaymentVouchers = await _context.PaymentVouchers
                .AnyAsync(p => p.BankAccountId == accountId);

            if (hasPaymentVouchers)
                return false;

            // Check receipt vouchers
            var hasReceiptVouchers = await _context.ReceiptVouchers
                .AnyAsync(r => r.BankAccountId == accountId);

            if (hasReceiptVouchers)
                return false;

            return true;
        }

        // ============================================================
        // ADD
        // ============================================================
        public async Task AddAsync(BankAccount entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            entity.IsActive = true;
            entity.IsDeleted = false;

            await _context.BankAccounts.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        // ============================================================
        // UPDATE
        // ============================================================
        public async Task UpdateAsync(BankAccount entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;

            _context.BankAccounts.Update(entity);

            await _context.SaveChangesAsync();
        }

        // ============================================================
        // SOFT DELETE  ← YEH METHOD FIX HUA HAI
        // ============================================================
        public async Task SoftDeleteAsync(
            int id,
            int companyId,
            string deletedBy)   // ✅ int → string (interface ke mutabiq)
        {
            var entity = await GetByIdAsync(id, companyId);

            if (entity == null)
                return;

            entity.IsDeleted = true;
            entity.DeletedBy = deletedBy;
            entity.DeletedDate = DateTime.UtcNow;

            await UpdateAsync(entity);
        }
    }
}