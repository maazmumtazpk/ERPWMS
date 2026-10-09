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

        public SalesService(
            ISalesMasterRepository salesMasterRepository,
            ISalesDetailRepository salesDetailRepository,
            IPickingListRepository pickingListRepository)
        {
            _salesMasterRepository = salesMasterRepository;
            _salesDetailRepository = salesDetailRepository;
            _pickingListRepository = pickingListRepository;
        }

        public async Task<IEnumerable<SalesMaster>> GetAllAsync(int companyId, bool includeDeleted = false)
        {
            return await _salesMasterRepository.GetAllAsync(companyId, includeDeleted);
        }

        public async Task<SalesMaster?> GetByIdAsync(int salesId, int companyId)
        {
            return await _salesMasterRepository.GetByIdAsync(salesId, companyId);
        }

        public async Task<SalesMaster?> GetByOrderNoAsync(string orderNo, int companyId)
        {
            return await _salesMasterRepository.GetByOrderNoAsync(orderNo, companyId);
        }

        public async Task<IEnumerable<SalesMaster>> GetByCustomerIdAsync(int customerId, int companyId)
        {
            return await _salesMasterRepository.GetByCustomerIdAsync(customerId, companyId);
        }

        public async Task<IEnumerable<SalesMaster>> GetByDateRangeAsync(DateTime? fromDate, DateTime? toDate, int companyId)
        {
            return await _salesMasterRepository.GetByDateRangeAsync(fromDate, toDate, companyId);
        }

        public async Task<IEnumerable<SalesMaster>> GetByStatusAsync(string status, int companyId)
        {
            return await _salesMasterRepository.GetByStatusAsync(status, companyId);
        }

        public async Task<SalesMaster> CreateSalesOrderAsync(
            SalesMaster sales,
            List<SalesDetail> details,
            string createdBy)
        {
            if (!await _salesMasterRepository.IsOrderNoUniqueAsync(sales.SalesOrderNo, sales.Warehouse?.Branch?.CompanyId ?? 0, null))
                throw new Exception("Sales Order Number already exists.");

            sales.Status = "Draft";
            sales.CreatedBy = createdBy.ToString();
            sales.CreatedDate = DateTime.UtcNow;

            await _salesMasterRepository.AddAsync(sales);

            foreach (var detail in details)
            {
                detail.SalesId = sales.SalesId;
                detail.CreatedBy = createdBy.ToString();
                detail.CreatedDate = DateTime.UtcNow;
                await _salesDetailRepository.AddAsync(detail);
            }

            return sales;
        }

        // ? FIXED — MISSING METHOD
        public async Task UpdateSalesOrderAsync(
            SalesMaster sales,
            List<SalesDetail> details,
            string updatedBy)
        {
            sales.UpdatedBy = updatedBy.ToString();
            sales.UpdatedDate = DateTime.UtcNow;

            await _salesMasterRepository.UpdateAsync(sales);

            var existingDetails = await _salesDetailRepository.GetBySalesIdAsync(sales.SalesId);

            foreach (var existing in existingDetails)
            {
                await _salesDetailRepository.DeleteAsync(existing.SalesDetailId);
            }

            foreach (var detail in details)
            {
                detail.SalesId = sales.SalesId;
                detail.CreatedBy = updatedBy.ToString();
                detail.CreatedDate = DateTime.UtcNow;
                await _salesDetailRepository.AddAsync(detail);
            }
        }

        // ? FIXED — MISSING METHOD
        public async Task<bool> DeleteSalesOrderAsync(
            int salesId,
            int companyId,
            string deletedBy)
        {
            if (!await CanBeDeletedAsync(salesId, companyId))
                throw new Exception("Cannot delete Sales Order as it may have been used.");

            await _salesMasterRepository.SoftDeleteAsync(salesId, companyId, deletedBy);
            return true;
        }

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

        public async Task<bool> IsOrderNoUniqueAsync(string orderNo, int companyId, int? excludeId = null)
        {
            return await _salesMasterRepository.IsOrderNoUniqueAsync(orderNo, companyId, excludeId);
        }

        public Task<decimal> CalculateTotalAmountAsync(List<SalesDetail> details)
        {
            decimal total = 0;
            foreach (var d in details)
            {
                total += (d.Quantity * d.UnitPrice) - (d.DiscountAmount ?? 0);
            }
            return Task.FromResult(total);
        }

        public async Task<bool> CanBeDeletedAsync(int salesId, int companyId)
        {
            return await _salesMasterRepository.CanBeDeletedAsync(salesId, companyId);
        }

        public async Task UpdateSalesOrderStatusAsync(
            int salesId,
            string status,
            int? workflowInstanceId = null,
            int? currentStageId = null)
        {
            await _salesMasterRepository.UpdateStatusAsync(salesId, status, workflowInstanceId, currentStageId);
        }

        public async Task<PickingList> GeneratePickingListAsync(int salesId, int warehouseId, string createdBy)
        {
            var sales = await _salesMasterRepository.GetByIdAsync(salesId, 0);
            if (sales == null)
                throw new Exception("Sales Order not found.");

            if (sales.Status != "Approved")
                throw new Exception("Only Approved Sales Orders can be picked.");

            var pickingNo = $"PICK-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";

            var picking = new PickingList
            {
                SalesId = sales.SalesId,
                WarehouseId = warehouseId,
                PickingNo = pickingNo,
                Status = "Draft",
                CreatedBy = createdBy.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            await _pickingListRepository.AddAsync(picking);
            return picking;
        }
    }
}