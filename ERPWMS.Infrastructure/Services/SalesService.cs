using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class SalesService : ISalesService
    {
        private readonly ISalesMasterRepository _salesMasterRepository;
        private readonly ISalesDetailRepository _salesDetailRepository;
        private readonly IPickingListRepository _pickingListRepository;
        private readonly ERPWMS.Application.Common.ICurrentUserService _currentUserService;

        public SalesService(
            ISalesMasterRepository salesMasterRepository,
            ISalesDetailRepository salesDetailRepository,
            IPickingListRepository pickingListRepository,
            ERPWMS.Application.Common.ICurrentUserService currentUserService)
        {
            _salesMasterRepository = salesMasterRepository;
            _salesDetailRepository = salesDetailRepository;
            _pickingListRepository = pickingListRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<SalesMaster>> GetAllAsync(int companyId, bool includeDeleted = false)
            => await _salesMasterRepository.GetAllAsync(companyId, includeDeleted);

        public async Task<SalesMaster?> GetByIdAsync(int salesId, int companyId)
            => await _salesMasterRepository.GetByIdAsync(salesId, companyId);

        public async Task<SalesMaster?> GetByOrderNoAsync(string orderNo, int companyId)
            => await _salesMasterRepository.GetByOrderNoAsync(orderNo, companyId);

        public async Task<IEnumerable<SalesMaster>> GetByCustomerIdAsync(int customerId, int companyId)
            => await _salesMasterRepository.GetByCustomerIdAsync(customerId, companyId);

        public async Task<IEnumerable<SalesMaster>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
            => await _salesMasterRepository.GetByDateRangeAsync(fromDate, toDate, companyId);

        public async Task<IEnumerable<SalesMaster>> GetByStatusAsync(string status, int companyId)
            => await _salesMasterRepository.GetByStatusAsync(status, companyId);

        // =========================================================
        // CREATE
        // =========================================================

        public async Task<SalesMaster> CreateSalesOrderAsync(
            SalesMaster sales,
            List<SalesDetail> details,
            string createdBy)
        {
            if (sales is null)
                throw new ArgumentNullException(nameof(sales));

            if (details is null || !details.Any())
                throw new ArgumentException("Sales order must have at least one detail line.", nameof(details));

            var companyId = sales.Warehouse?.Branch?.CompanyId ?? 0;
            if (companyId <= 0)
                throw new ArgumentException("Sales order must have a valid company.");

            if (!await _salesMasterRepository.IsOrderNoUniqueAsync(sales.SalesOrderNo, companyId, null))
                throw new InvalidOperationException("Sales Order Number already exists.");

            foreach (var detail in details)
            {
                ValidateSalesDetail(detail);
            }

            sales.Status = "Draft";
            sales.CreatedBy = createdBy;
            sales.CreatedDate = DateTime.UtcNow;

            await _salesMasterRepository.AddAsync(sales);

            foreach (var detail in details)
            {
                detail.SalesId = sales.SalesId;
                detail.CreatedBy = createdBy;
                detail.CreatedDate = DateTime.UtcNow;
                await _salesDetailRepository.AddAsync(detail);
            }

            return sales;
        }

        private static void ValidateSalesDetail(SalesDetail detail)
        {
            if (detail is null)
                throw new ArgumentException("Sales detail cannot be null.");

            if (detail.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            if (detail.UnitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative.");

            if ((detail.DiscountAmount ?? 0) < 0)
                throw new ArgumentException("Discount cannot be negative.");

            if ((detail.DiscountAmount ?? 0) > (detail.Quantity * detail.UnitPrice))
                throw new ArgumentException("Discount cannot exceed line total.");
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task UpdateSalesOrderAsync(
            SalesMaster sales,
            List<SalesDetail> details,
            string updatedBy)
        {
            if (sales is null)
                throw new ArgumentNullException(nameof(sales));

            sales.UpdatedBy = updatedBy;
            sales.UpdatedDate = DateTime.UtcNow;

            await _salesMasterRepository.UpdateAsync(sales);

            var existingDetails = await _salesDetailRepository.GetBySalesIdAsync(sales.SalesId);

            foreach (var existing in existingDetails)
            {
                await _salesDetailRepository.DeleteAsync(existing.SalesDetailId);
            }

            if (details != null)
            {
                foreach (var detail in details)
                {
                    detail.SalesId = sales.SalesId;
                    detail.CreatedBy = updatedBy;
                    detail.CreatedDate = DateTime.UtcNow;
                    await _salesDetailRepository.AddAsync(detail);
                }
            }
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool> DeleteSalesOrderAsync(
            int salesId,
            int companyId,
            string deletedBy)
        {
            if (!await CanBeDeletedAsync(salesId, companyId))
                throw new InvalidOperationException("Cannot delete Sales Order as it may have been used.");

            await _salesMasterRepository.SoftDeleteAsync(salesId, companyId, deletedBy);
            return true;
        }

        // =========================================================
        // WORKFLOW
        // =========================================================

        public async Task<bool> SubmitSalesOrderAsync(int salesId, int companyId, int userId)
        {
            var sales = await _salesMasterRepository.GetByIdAsync(salesId, companyId);
            if (sales == null) return false;

            sales.Status = "Submitted";
            sales.UpdatedBy = userId.ToString();
            sales.UpdatedDate = DateTime.UtcNow;

            await _salesMasterRepository.UpdateAsync(sales);
            return true;
        }

        public async Task<bool> ApproveSalesOrderAsync(int salesId, int companyId, int userId)
        {
            var sales = await _salesMasterRepository.GetByIdAsync(salesId, companyId);
            if (sales == null) return false;

            sales.Status = "Approved";
            sales.UpdatedBy = userId.ToString();
            sales.UpdatedDate = DateTime.UtcNow;

            await _salesMasterRepository.UpdateAsync(sales);
            return true;
        }

        public async Task<bool> RejectSalesOrderAsync(int salesId, int companyId, int userId)
        {
            var sales = await _salesMasterRepository.GetByIdAsync(salesId, companyId);
            if (sales == null) return false;

            sales.Status = "Rejected";
            sales.UpdatedBy = userId.ToString();
            sales.UpdatedDate = DateTime.UtcNow;

            await _salesMasterRepository.UpdateAsync(sales);
            return true;
        }

        // =========================================================
        // CHECKS
        // =========================================================

        public async Task<bool> IsOrderNoUniqueAsync(string orderNo, int companyId, int? excludeId = null)
            => await _salesMasterRepository.IsOrderNoUniqueAsync(orderNo, companyId, excludeId);

        public Task<decimal> CalculateTotalAmountAsync(List<SalesDetail> details)
        {
            if (details is null || !details.Any())
                return Task.FromResult(0m);

            decimal total = 0;
            foreach (var d in details)
            {
                total += (d.Quantity * d.UnitPrice) - (d.DiscountAmount ?? 0);
            }
            return Task.FromResult(total);
        }

        public async Task<bool> CanBeDeletedAsync(int salesId, int companyId)
            => await _salesMasterRepository.CanBeDeletedAsync(salesId, companyId);

        public async Task UpdateSalesOrderStatusAsync(
            int salesId,
            string status,
            int? workflowInstanceId = null,
            int? currentStageId = null)
        {
            await _salesMasterRepository.UpdateStatusAsync(salesId, status, workflowInstanceId, currentStageId);
        }

        // =========================================================
        // PICKING LIST
        // =========================================================

        public async Task<PickingList> GeneratePickingListAsync(int salesId, int warehouseId, string createdBy)
        {
            var sales = await _salesMasterRepository.GetByIdAsync(salesId, _currentUserService.GetCompanyId());
            if (sales == null)
                throw new InvalidOperationException("Sales Order not found.");

            if (sales.Status != "Approved")
                throw new InvalidOperationException("Only Approved Sales Orders can be picked.");

            // Use GUID to avoid collision
            var pickingNo = $"PICK-{DateTime.Now:yyyyMMdd}-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";

            var picking = new PickingList
            {
                SalesId = sales.SalesId,
                WarehouseId = warehouseId,
                PickingNo = pickingNo,
                Status = "Draft",
                CreatedBy = createdBy,
                CreatedDate = DateTime.UtcNow
            };

            await _pickingListRepository.AddAsync(picking);
            return picking;
        }
    }
}


