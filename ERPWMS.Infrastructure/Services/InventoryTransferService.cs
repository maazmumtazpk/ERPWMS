using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
            if (transfer is null)
                throw new ArgumentNullException(nameof(transfer));

            if (details is null || !details.Any())
                throw new ArgumentException("Transfer must have at least one detail.", nameof(details));

            if (transfer.FromWarehouseId == transfer.ToWarehouseId)
                throw new InvalidOperationException("Source and destination warehouse cannot be the same.");

            if (!await _stockTransferRepository.IsTransferNoUniqueAsync(transfer.TransferNo, null))
                throw new InvalidOperationException("Transfer Number already exists.");

            // Validate stock for each detail
            foreach (var detail in details)
            {
                if (detail.Quantity <= 0)
                    throw new InvalidOperationException($"Invalid quantity for product {detail.ProductId}.");

                var currentBalance = await _stockLedgerRepository.GetCurrentBalanceAsync(
                    detail.ProductId, transfer.FromWarehouseId, detail.FromBinId);

                if (currentBalance < detail.Quantity)
                    throw new InvalidOperationException(
                        $"Insufficient stock for product {detail.ProductId}. " +
                        $"Available: {currentBalance}, Requested: {detail.Quantity}");
            }

            transfer.Status = "Draft";
            transfer.CreatedBy = createdBy;
            transfer.CreatedDate = DateTime.UtcNow;

            await _stockTransferRepository.AddAsync(transfer);

            foreach (var detail in details)
            {
                detail.TransferId = transfer.TransferId;
                detail.CreatedBy = createdBy;
                detail.CreatedDate = DateTime.UtcNow;
                await _stockTransferDetailRepository.AddAsync(detail);
            }

            return transfer;
        }

        public async Task<StockTransfer> UpdateTransferAsync(
            StockTransfer transfer,
            List<StockTransferDetail> details,
            string updatedBy)
        {
            if (transfer is null)
                throw new ArgumentNullException(nameof(transfer));

            if (transfer.Status != "Draft")
                throw new InvalidOperationException("Only Draft transfers can be updated.");

            transfer.UpdatedBy = updatedBy;
            transfer.UpdatedDate = DateTime.UtcNow;

            await _stockTransferRepository.UpdateAsync(transfer);

            // Delete existing details
            var existingDetails = await _stockTransferDetailRepository
                .GetByTransferIdAsync(transfer.TransferId);

            foreach (var existing in existingDetails)
            {
                await _stockTransferDetailRepository.DeleteAsync(existing.TransferDetailId);
            }

            // Add new details
            if (details != null && details.Any())
            {
                foreach (var detail in details)
                {
                    if (detail.Quantity <= 0)
                        throw new InvalidOperationException($"Invalid quantity for product {detail.ProductId}.");

                    detail.TransferId = transfer.TransferId;
                    detail.CreatedBy = updatedBy;
                    detail.CreatedDate = DateTime.UtcNow;
                    await _stockTransferDetailRepository.AddAsync(detail);
                }
            }

            return transfer;
        }

        public async Task<bool> DeleteTransferAsync(int transferId, string deletedBy)
        {
            var transfer = await _stockTransferRepository.GetByIdAsync(transferId);
            if (transfer == null) return false;

            if (transfer.Status != "Draft" && transfer.Status != "Rejected")
                throw new InvalidOperationException("Only Draft or Rejected transfers can be deleted.");

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

            if (transfer.Status != "Draft")
                throw new InvalidOperationException("Only Draft transfers can be submitted.");

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

            if (transfer.Status != "Submitted")
                throw new InvalidOperationException("Only Submitted transfers can be approved.");

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

            if (transfer.Status != "Submitted")
                throw new InvalidOperationException("Only Submitted transfers can be rejected.");

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

            if (transfer.Status != "Approved")
                throw new InvalidOperationException("Only Approved transfers can be dispatched.");

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

            if (transfer.Status != "InTransit")
                throw new InvalidOperationException("Only InTransit transfers can be received.");

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
                throw new InvalidOperationException("Source and destination warehouse cannot be the same.");

            if (details == null || !details.Any())
                throw new InvalidOperationException("Transfer must have at least one detail.");

            foreach (var detail in details)
            {
                if (detail.Quantity <= 0)
                    throw new InvalidOperationException($"Invalid quantity for product {detail.ProductId}.");
            }

            await Task.CompletedTask;
        }

        public async Task<StockLedger> CreateLedgerEntryForTransferOutAsync(
            StockTransfer transfer, int userId)
        {
            var ledger = new StockLedger
            {
                ProductId = transfer.ProductId,
                WarehouseId = transfer.FromWarehouseId,
                Quantity = -transfer.Quantity,
                TransactionType = "TransferOut",
                ReferenceId = transfer.TransferId,
                ReferenceDocNo = transfer.TransferId.ToString(),
                ReferenceDocType = "StockTransfer",
                CreatedBy = userId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            return await _stockLedgerRepository.AddAsync(ledger);
        }

        public async Task<StockLedger> CreateLedgerEntryForTransferInAsync(
            StockTransfer transfer, int userId)
        {
            var ledger = new StockLedger
            {
                ProductId = transfer.ProductId,
                WarehouseId = transfer.ToWarehouseId,
                Quantity = transfer.Quantity,
                TransactionType = "TransferIn",
                ReferenceId = transfer.TransferId,
                ReferenceDocNo = transfer.TransferId.ToString(),
                ReferenceDocType = "StockTransfer",
                CreatedBy = userId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            return await _stockLedgerRepository.AddAsync(ledger);
        }
    }
}

