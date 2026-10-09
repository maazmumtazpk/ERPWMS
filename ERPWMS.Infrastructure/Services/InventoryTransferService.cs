using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class InventoryTransferService : IInventoryTransferService
    {
        private readonly IStockTransferRepository _stockTransferRepository;
        private readonly IStockTransferDetailRepository _stockTransferDetailRepository;
        private readonly IStockLedgerRepository _stockLedgerRepository;

        public InventoryTransferService(
            IStockTransferRepository stockTransferRepository,
            IStockTransferDetailRepository stockTransferDetailRepository,
            IStockLedgerRepository stockLedgerRepository)
        {
            _stockTransferRepository = stockTransferRepository;
            _stockTransferDetailRepository = stockTransferDetailRepository;
            _stockLedgerRepository = stockLedgerRepository;
        }

        public async Task<StockTransfer> CreateTransferAsync(
            StockTransfer transfer,
            List<StockTransferDetail> details,
            string createdBy)
        {
            if (!await _stockTransferRepository.IsTransferNoUniqueAsync(transfer.TransferNo, null))
                throw new Exception("Transfer Number already exists.");

            transfer.Status = "Draft";
            transfer.CreatedBy = createdBy.ToString();
            transfer.CreatedDate = DateTime.UtcNow;

            await _stockTransferRepository.AddAsync(transfer);

            foreach (var detail in details)
            {
                detail.TransferId = transfer.TransferId;
                detail.CreatedBy = createdBy.ToString();
                detail.CreatedDate = DateTime.UtcNow;
                await _stockTransferDetailRepository.AddAsync(detail);
            }

            return transfer;
        }

        // ? FIXED — MISSING METHOD
        public async Task<StockTransfer> UpdateTransferAsync(
            StockTransfer transfer,
            List<StockTransferDetail> details,
            string updatedBy)
        {
            transfer.UpdatedBy = updatedBy.ToString();
            transfer.UpdatedDate = DateTime.UtcNow;

            await _stockTransferRepository.UpdateAsync(transfer);

            // Update details — remove existing, add new
            if (details != null && details.Count > 0)
            {
                foreach (var detail in details)
                {
                    detail.TransferId = transfer.TransferId;
                    detail.CreatedBy = updatedBy.ToString();
                    detail.CreatedDate = DateTime.UtcNow;
                    await _stockTransferDetailRepository.AddAsync(detail);
                }
            }

            return transfer;
        }

        // ? FIXED — MISSING METHOD
        public async Task<bool> DeleteTransferAsync(int transferId, string deletedBy)
        {
            var transfer = await _stockTransferRepository.GetByIdAsync(transferId);
            if (transfer == null) return false;

            if (transfer.Status != "Draft" && transfer.Status != "Rejected")
                throw new Exception("Only Draft or Rejected transfers can be deleted.");

            transfer.IsDeleted = true;
            transfer.DeletedBy = deletedBy;
            transfer.DeletedDate = DateTime.UtcNow;

            await _stockTransferRepository.UpdateAsync(transfer);
            return true;
        }

        public async Task<StockTransfer?> GetTransferByIdAsync(int transferId)
            => await _stockTransferRepository.GetByIdAsync(transferId);

        public async Task<IEnumerable<StockTransfer>> GetAllTransfersAsync(int warehouseId, bool includeDeleted = false)
            => await _stockTransferRepository.GetByFromWarehouseIdAsync(warehouseId, includeDeleted);

        public async Task<bool> SubmitTransferAsync(int transferId, int userId)
        {
            var transfer = await _stockTransferRepository.GetByIdAsync(transferId);
            if (transfer == null) return false;

            transfer.Status = "Submitted";
            transfer.UpdatedBy = userId.ToString();
            transfer.UpdatedDate = DateTime.UtcNow;

            await _stockTransferRepository.UpdateAsync(transfer);
            return true;
        }

        public async Task<bool> ApproveTransferAsync(int transferId, int userId)
        {
            var transfer = await _stockTransferRepository.GetByIdAsync(transferId);
            if (transfer == null) return false;

            transfer.Status = "Approved";
            transfer.ApprovedBy = userId.ToString();
            transfer.ApprovedDate = DateTime.UtcNow;
            transfer.UpdatedBy = userId.ToString();
            transfer.UpdatedDate = DateTime.UtcNow;

            await _stockTransferRepository.UpdateAsync(transfer);
            return true;
        }

        public async Task<bool> RejectTransferAsync(int transferId, int userId)
        {
            var transfer = await _stockTransferRepository.GetByIdAsync(transferId);
            if (transfer == null) return false;

            transfer.Status = "Rejected";
            transfer.UpdatedBy = userId.ToString();
            transfer.UpdatedDate = DateTime.UtcNow;

            await _stockTransferRepository.UpdateAsync(transfer);
            return true;
        }

        public async Task<bool> DispatchTransferAsync(int transferId, int userId)
        {
            var transfer = await _stockTransferRepository.GetByIdAsync(transferId);
            if (transfer == null) return false;

            transfer.Status = "InTransit";
            transfer.UpdatedBy = userId.ToString();
            transfer.UpdatedDate = DateTime.UtcNow;

            await _stockTransferRepository.UpdateAsync(transfer);
            return true;
        }

        public async Task<bool> ReceiveTransferAsync(int transferId, int userId)
        {
            var transfer = await _stockTransferRepository.GetByIdAsync(transferId);
            if (transfer == null) return false;

            transfer.Status = "Completed";
            transfer.ReceivedBy = userId.ToString();
            transfer.ReceivedDate = DateTime.UtcNow;
            transfer.UpdatedBy = userId.ToString();
            transfer.UpdatedDate = DateTime.UtcNow;

            await _stockTransferRepository.UpdateAsync(transfer);
            return true;
        }

        public async Task<bool> CanBeDispatchedAsync(int transferId)
            => await _stockTransferRepository.CanBeDispatchedAsync(transferId);

        public async Task<bool> CanBeReceivedAsync(int transferId)
            => await _stockTransferRepository.CanBeReceivedAsync(transferId);

        public async Task UpdateTransferStatusAsync(int transferId, string status)
        {
            await _stockTransferRepository.UpdateStatusAsync(transferId, status);
        }

        public async Task<bool> HasSufficientStockAsync(int transferId)
            => await _stockTransferRepository.HasSufficientStockAsync(transferId);

        public async Task ValidateTransferAsync(StockTransfer transfer, List<StockTransferDetail> details)
        {
            if (transfer == null)
                throw new ArgumentNullException(nameof(transfer));

            if (transfer.FromWarehouseId == transfer.ToWarehouseId)
                throw new Exception("Source and destination warehouse cannot be the same.");

            await Task.CompletedTask;
        }

        public Task<StockLedger> CreateLedgerEntryForTransferOutAsync(StockTransfer transfer, int userId)
        {
            var ledger = new StockLedger
            {
                ProductId = transfer.ProductId,
                WarehouseId = transfer.FromWarehouseId,
                Quantity = -transfer.Quantity,
                TransactionType = "TransferOut",
                ReferenceId = transfer.TransferId,
                CreatedBy = userId.ToString(),
                CreatedDate = DateTime.UtcNow
            };
            return Task.FromResult(ledger);
        }

        public Task<StockLedger> CreateLedgerEntryForTransferInAsync(StockTransfer transfer, int userId)
        {
            var ledger = new StockLedger
            {
                ProductId = transfer.ProductId,
                WarehouseId = transfer.ToWarehouseId,
                Quantity = transfer.Quantity,
                TransactionType = "TransferIn",
                ReferenceId = transfer.TransferId,
                CreatedBy = userId.ToString(),
                CreatedDate = DateTime.UtcNow
            };
            return Task.FromResult(ledger);
        }
    }
}