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
    public class InvoiceRepository(AppDbContext context) : IInvoiceRepository
    {
        public async Task<IEnumerable<Invoice>> GetAllAsync(int companyId, bool includeDeleted = false)
        {
            var query = context.Invoices
                .Include(i => i.Sales)
                .Where(i => i.Sales.Warehouse.Branch.CompanyId == companyId);

            if (!includeDeleted) query = query.Where(i => i.IsDeleted != true);

            return await query.OrderByDescending(i => i.InvoiceId).ToListAsync();
        }

        public async Task<Invoice?> GetByIdAsync(int invoiceId, int companyId)
        {
            return await context.Invoices
                .Include(i => i.Sales)
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i =>
                    i.InvoiceId == invoiceId &&
                    i.Sales.Warehouse.Branch.CompanyId == companyId);
        }

        public async Task<Invoice?> GetByInvoiceNoAsync(string invoiceNo, int companyId)
        {
            return await context.Invoices
                .Include(i => i.Sales)
                .FirstOrDefaultAsync(i =>
                    i.InvoiceNo == invoiceNo &&
                    i.Sales.Warehouse.Branch.CompanyId == companyId &&
                    i.IsDeleted != true);
        }

        public async Task<IEnumerable<Invoice>> GetBySalesIdAsync(int salesId)
        {
            return await context.Invoices
                .Where(i => i.SalesId == salesId && i.IsDeleted != true)
                .OrderByDescending(i => i.InvoiceId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByStatusAsync(string status, int companyId)
        {
            return await context.Invoices
                .Where(i => i.Status == status &&
                            i.Sales.Warehouse.Branch.CompanyId == companyId &&
                            i.IsDeleted != true)
                .OrderByDescending(i => i.InvoiceId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
        {
            var query = context.Invoices
                .Where(i => i.Sales.Warehouse.Branch.CompanyId == companyId && i.IsDeleted != true);

            if (fromDate.HasValue) query = query.Where(i => i.InvoiceDate >= fromDate);
            if (toDate.HasValue) query = query.Where(i => i.InvoiceDate <= toDate);

            return await query.OrderByDescending(i => i.InvoiceDate).ToListAsync();
        }

        public async Task<bool> IsInvoiceNoUniqueAsync(string invoiceNo, int companyId, int? excludeId = null)
        {
            var query = context.Invoices
                .Where(i => i.InvoiceNo == invoiceNo && i.IsDeleted != true);

            if (excludeId.HasValue) query = query.Where(i => i.InvoiceId != excludeId);

            return !await query.AnyAsync();
        }

        public async Task AddAsync(Invoice entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            await context.Invoices.AddAsync(entity);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Invoice entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            context.Invoices.Update(entity);
            await context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int invoiceId, int companyId, string deletedBy)
        {
            var entity = await GetByIdAsync(invoiceId, companyId);
            if (entity is null) return;

            entity.IsDeleted = true;
            entity.DeletedBy = deletedBy;
            entity.DeletedDate = DateTime.UtcNow;

            await UpdateAsync(entity);
        }

        public async Task UpdateStatusAsync(
            int invoiceId,
            string status,
            int? workflowInstanceId = null,
            int? currentStageId = null)
        {
            var entity = await context.Invoices.FindAsync(invoiceId);
            if (entity is null) return;

            entity.Status = status;
            if (workflowInstanceId.HasValue) entity.WorkflowInstanceId = workflowInstanceId;
            if (currentStageId.HasValue) entity.CurrentStageId = currentStageId;
            entity.UpdatedDate = DateTime.UtcNow;

            await context.SaveChangesAsync();
        }

        public async Task<bool> CanBeDeletedAsync(int invoiceId, int companyId)
        {
            var entity = await GetByIdAsync(invoiceId, companyId);
            if (entity is null || entity.IsDeleted == true) return false;

            var hasReturn = await context.SalesReturns
                .AnyAsync(r => r.InvoiceId == invoiceId && r.IsDeleted != true);

            return !hasReturn;
        }

        public async Task<decimal> GetTotalPaidAsync(int invoiceId)
        {
            var invoice = await context.Invoices.FindAsync(invoiceId);
            if (invoice is null) return 0m;

            // ✅ Robust: nullable aur non-nullable dono handle karta hai
            decimal netAmount = (decimal?)invoice.NetAmount ?? 0m;
            decimal balanceDue = (decimal?)invoice.BalanceDue ?? 0m;

            return netAmount - balanceDue;
        }

        public async Task UpdateBalanceDueAsync(int invoiceId)
        {
            var invoice = await context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == invoiceId);

            if (invoice is null) return;

            // ✅ Robust: nullable-safe calculation
            decimal netAmount = (decimal?)invoice.NetAmount ?? 0m;
            invoice.BalanceDue = netAmount - await GetTotalPaidAsync(invoiceId);

            await context.SaveChangesAsync();
        }
    }
}