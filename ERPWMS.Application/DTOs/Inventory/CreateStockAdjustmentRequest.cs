using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class CreateStockAdjustmentRequest
    {
        [Required(ErrorMessage = "Warehouse is required.")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Bin is required.")]
        public int BinId { get; set; }

        [Required(ErrorMessage = "Adjustment Type is required.")]
        [StringLength(20)]
        public string AdjustmentType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [StringLength(500)]
        public string Reason { get; set; } = string.Empty;

        public string? Remarks { get; set; }

        public List<CreateStockAdjustmentDetailRequest>? AdjustmentDetails { get; set; }
    }

    public class CreateStockAdjustmentDetailRequest
    {
        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Bin is required.")]
        public int BinId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        public int PreviousQty { get; set; }
        public int NewQty { get; set; }

        public string? BatchNo { get; set; }
        public DateOnly? ExpiryDate { get; set; }
    }
}