using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services
{
    public class DispatchService : IDispatchService
    {
        private readonly IDispatchMasterRepository _dispatchMasterRepository;
        private readonly IDispatchDetailRepository _dispatchDetailRepository;
        private readonly ISalesDetailRepository _salesDetailRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceDetailRepository _invoiceDetailRepository;
        private readonly AppDbContext _context;

        public DispatchService(
            IDispatchMasterRepository dispatchMasterRepository,
            IDispatchDetailRepository dispatchDetailRepository,
            ISalesDetailRepository salesDetailRepository,
            IInvoiceRepository invoiceRepository,
            IInvoiceDetailRepository invoiceDetailRepository,
            AppDbContext context)
        {
            _dispatchMasterRepository = dispatchMasterRepository;
            _dispatchDetailRepository = dispatchDetailRepository;
            _salesDetailRepository = salesDetailRepository;
            _invoiceRepository = invoiceRepository;
            _invoiceDetailRepository = invoiceDetailRepository;
            _context = context;
        }

        public async Task<IEnumerable<DispatchMaster>> GetAllAsync(int companyId, bool includeDeleted = false)
            => await _dispatchMasterRepository.GetAllAsync(companyId, includeDeleted);

        public async Task<DispatchMaster?> GetByIdAsync(int dispatchId, int companyId)
            => await _dispatchMasterRepository.GetByIdAsync(dispatchId, companyId);

        public async Task<DispatchMaster?> GetByDispatchNoAsync(string dispatchNo, int companyId)
            => await _dispatchMasterRepository.GetByDispatchNoAsync(dispatchNo, companyId);

        public async Task<IEnumerable<DispatchMaster>> GetBySalesIdAsync(int salesId)
            => await _dispatchMasterRepository.GetBySalesIdAsync(salesId);

        public async Task<IEnumerable<DispatchMaster>> GetByStatusAsync(string status, int companyId)
            => await _dispatchMasterRepository.GetByStatusAsync(status, companyId);

        public async Task<IEnumerable<DispatchMaster>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
            => await _dispatchMasterRepository.GetByDateRangeAsync(fromDate, toDate, companyId);

        public async Task<DispatchMaster> CreateDispatchAsync(
            DispatchMaster dispatch,
            List<DispatchDetail> details,
            string createdBy)
        {
            if (!await _dispatchMasterRepository.IsDispatchNoUniqueAsync(dispatch.DispatchNo, dispatch.Warehouse?.Branch?.CompanyId ?? 0, null))
                throw new Exception("Dispatch Number already exists.");

            dispatch.Status = "Draft";
            dispatch.CreatedBy = createdBy.ToString();
            dispatch.CreatedDate = DateTime.UtcNow;

            await _dispatchMasterRepository.AddAsync(dispatch);

            foreach (var detail in details)
            {
                detail.DispatchId = dispatch.DispatchId;
                detail.CreatedBy = createdBy.ToString();
                detail.CreatedDate = DateTime.UtcNow;
                await _dispatchDetailRepository.AddAsync(detail);
            }

            return dispatch;
        }

        // ? FIXED — MISSING METHOD
        public async Task UpdateDispatchAsync(
            DispatchMaster dispatch,
            List<DispatchDetail> details,
            string updatedBy)
        {
            dispatch.UpdatedBy = updatedBy.ToString();
            dispatch.UpdatedDate = DateTime.UtcNow;

            await _dispatchMasterRepository.UpdateAsync(dispatch);

            var existingDetails = await _dispatchDetailRepository.GetByDispatchIdAsync(dispatch.DispatchId);

            foreach (var existing in existingDetails)
            {
                await _dispatchDetailRepository.DeleteAsync(existing.DispatchDetailId);
            }

            foreach (var detail in details)
            {
                detail.DispatchId = dispatch.DispatchId;
                detail.CreatedBy = updatedBy.ToString();
                detail.CreatedDate = DateTime.UtcNow;
                await _dispatchDetailRepository.AddAsync(detail);
            }
        }

        // ? FIXED — MISSING METHOD
        public async Task<bool> DeleteDispatchAsync(
            int dispatchId,
            int companyId,
            string deletedBy)
        {
            if (!await CanBeDeletedAsync(dispatchId, companyId))
                throw new Exception("Cannot delete Dispatch as it may have associated Invoice.");

            await _dispatchMasterRepository.SoftDeleteAsync(dispatchId, companyId, deletedBy);
            return true;
        }

        public async Task<bool> DispatchOrderAsync(int dispatchId, string dispatchedBy)
        {
            var dispatch = await _dispatchMasterRepository.GetByIdAsync(dispatchId, 0);
            if (dispatch == null) return false;

            if (dispatch.Status != "Draft")
                throw new Exception("Only Draft dispatches can be dispatched.");

            dispatch.Status = "Dispatched";
            dispatch.DispatchedBy = dispatchedBy;
            dispatch.DispatchDate = DateTime.UtcNow;
            dispatch.UpdatedBy = dispatchedBy;
            dispatch.UpdatedDate = DateTime.UtcNow;

            await _dispatchMasterRepository.UpdateAsync(dispatch);

            var details = await _dispatchDetailRepository.GetByDispatchIdAsync(dispatchId);
            foreach (var detail in details)
            {
                var salesDetail = await _salesDetailRepository.GetByIdAsync(detail.SalesDetailId);
                if (salesDetail != null)
                {
                    salesDetail.ShippedQty = (salesDetail.ShippedQty ?? 0) + detail.Quantity;
                    await _salesDetailRepository.UpdateAsync(salesDetail);
                }
            }

            return true;
        }

        public async Task<bool> UpdateTrackingAsync(int dispatchId, string trackingNo)
        {
            var dispatch = await _dispatchMasterRepository.GetByIdAsync(dispatchId, 0);
            if (dispatch == null) return false;

            dispatch.TrackingNo = trackingNo;
            dispatch.UpdatedDate = DateTime.UtcNow;
            await _dispatchMasterRepository.UpdateAsync(dispatch);
            return true;
        }

        public async Task<bool> ConfirmDeliveryAsync(int dispatchId, DateTime deliveryDate)
        {
            var dispatch = await _dispatchMasterRepository.GetByIdAsync(dispatchId, 0);
            if (dispatch == null) return false;

            if (dispatch.Status != "Dispatched")
                throw new Exception("Only Dispatched orders can be confirmed delivered.");

            dispatch.Status = "Delivered";
            dispatch.DeliveryDate = deliveryDate;
            dispatch.UpdatedDate = DateTime.UtcNow;

            await _dispatchMasterRepository.UpdateAsync(dispatch);
            return true;
        }

        public async Task<bool> IsDispatchNoUniqueAsync(string dispatchNo, int companyId, int? excludeId = null)
            => await _dispatchMasterRepository.IsDispatchNoUniqueAsync(dispatchNo, companyId, excludeId);

        public async Task<bool> CanBeDeletedAsync(int dispatchId, int companyId)
            => await _dispatchMasterRepository.CanBeDeletedAsync(dispatchId, companyId);

        public async Task UpdateDispatchStatusAsync(int dispatchId, string status, int? workflowInstanceId = null, int? currentStageId = null)
        {
            await _dispatchMasterRepository.UpdateStatusAsync(dispatchId, status, workflowInstanceId, currentStageId);
        }

        public async Task<Invoice> GenerateInvoiceFromDispatchAsync(int dispatchId, string createdBy)
        {
            var dispatch = await _dispatchMasterRepository.GetByIdAsync(dispatchId, 0);
            if (dispatch == null)
                throw new Exception("Dispatch not found.");

            if (dispatch.Status != "Dispatched")
                throw new Exception("Only dispatched orders can be invoiced.");

            var invoiceNo = $"INV-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

            var invoice = new Invoice
            {
                SalesId = dispatch.SalesId,
                InvoiceNo = invoiceNo,
                InvoiceDate = DateTime.UtcNow,
                TotalAmount = 0,
                Discount = 0,
                TaxAmount = 0,
                NetAmount = 0,
                BalanceDue = 0,
                Status = "Draft",
                DueDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30)),
                CreatedBy = createdBy.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            var dispatchDetails = await _dispatchDetailRepository.GetByDispatchIdAsync(dispatchId);

            decimal total = 0;
            var invoiceDetails = new List<InvoiceDetail>();

            foreach (var dd in dispatchDetails)
            {
                var salesDetail = await _salesDetailRepository.GetByIdAsync(dd.SalesDetailId);
                if (salesDetail == null) continue;

                var unitPrice = salesDetail.UnitPrice;
                var lineTotal = dd.Quantity * unitPrice;
                total += lineTotal;

                invoiceDetails.Add(new InvoiceDetail
                {
                    SalesDetailId = dd.SalesDetailId,
                    ProductId = dd.ProductId,
                    Quantity = dd.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = lineTotal,
                    DiscountPercent = salesDetail.DiscountPercent,
                    CreatedBy = createdBy.ToString(),
                    CreatedDate = DateTime.UtcNow
                });
            }

            invoice.TotalAmount = total;
            invoice.NetAmount = total;
            invoice.BalanceDue = total;

            await _invoiceRepository.AddAsync(invoice);

            foreach (var invd in invoiceDetails)
            {
                invd.InvoiceId = invoice.InvoiceId;
                await _invoiceDetailRepository.AddAsync(invd);
            }

            return invoice;
        }
    }
}