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
    public class TaxSettingRepository : ITaxSettingRepository
    {
        private readonly AppDbContext _context;

        public TaxSettingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<TaxSetting>> GetAllAsync(int companyId, bool includeDeleted = false)
        {
            var query = _context.TaxSettings
                .Where(t => t.CompanyId == companyId);

            if (!includeDeleted) query = query.Where(t => t.IsDeleted != true);

            return await query.OrderBy(t => t.TaxName).ToListAsync();
        }

        public async Task<TaxSetting?> GetByIdAsync(int id, int companyId)
        {
            return await _context.TaxSettings
                .FirstOrDefaultAsync(t =>
                    t.TaxId == id &&
                    t.CompanyId == companyId &&
                    t.IsDeleted != true);
        }

        public async Task<TaxSetting?> GetByCodeAsync(int companyId, string taxCode)
        {
            return await _context.TaxSettings
                .FirstOrDefaultAsync(t =>
                    t.CompanyId == companyId &&
                    t.TaxCode == taxCode &&
                    t.IsDeleted != true);
        }

        public async Task<TaxSetting?> GetDefaultTaxAsync(int companyId)
        {
            return await _context.TaxSettings
                .FirstOrDefaultAsync(t =>
                    t.CompanyId == companyId &&
                    t.IsDefault == true &&
                    t.IsDeleted != true);
        }

        public async Task<bool> HasDefaultTaxAsync(int companyId, int? excludeId = null)
        {
            var query = _context.TaxSettings
                .Where(t => t.CompanyId == companyId &&
                            t.IsDefault == true &&
                            t.IsDeleted != true);

            if (excludeId.HasValue) query = query.Where(t => t.TaxId != excludeId);

            return await query.AnyAsync();
        }

        public async Task<bool> IsCodeExistsAsync(int companyId, string taxCode, int? excludeId = null)
        {
            var query = _context.TaxSettings
                .Where(t => t.CompanyId == companyId &&
                            t.TaxCode == taxCode &&
                            t.IsDeleted != true);

            if (excludeId.HasValue) query = query.Where(t => t.TaxId != excludeId);

            return await query.AnyAsync();
        }

        public async Task AddAsync(TaxSetting entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await _context.TaxSettings.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TaxSetting entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _context.TaxSettings.Update(entity);
            await _context.SaveChangesAsync();
        }

        // ✅ FIXED: DeletedBy / DeletedDate properties remove kar di
        public async Task SoftDeleteAsync(int id, int companyId, string deletedBy)
        {
            var entity = await GetByIdAsync(id, companyId);
            if (entity == null) return;

            entity.IsDeleted = true;
            // Note: TaxSetting model mein DeletedBy/DeletedDate columns nahi hain
            // Agar aap chahein to add kar sakte ho database mein

            await UpdateAsync(entity);
        }
    }
}