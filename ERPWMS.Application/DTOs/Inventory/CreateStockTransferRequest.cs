using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class CreateStockTransferRequest
    {
        [Required(ErrorMessage = "From Warehouse is required.")]
        public int FromWarehouseId { get; set; }

        [Required(ErrorMessage = "To Warehouse is required.")]
        public int ToWarehouseId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Transfer Number is required.")]
        [StringLength(50)]
        public string TransferNo { get; set; } = string.Empty;

        public DateTime? TransferDate { get; set; }

        public string? Remarks { get; set; }

        public List<CreateStockTransferDetailRequest>? TransferDetails { get; set; }
    }

    public class CreateStockTransferDetailRequest
    {
        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "From Bin is required.")]
        public int FromBinId { get; set; }

        [Required(ErrorMessage = "To Bin is required.")]
        public int ToBinId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        public string? BatchNo { get; set; }
        public DateOnly? ExpiryDate { get; set; }
    }
}