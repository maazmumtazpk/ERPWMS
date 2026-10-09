using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class CreateCycleCountRequest
    {
        [Required(ErrorMessage = "Warehouse is required.")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Bin is required.")]
        public int BinId { get; set; }

        public string? Remarks { get; set; }

        public List<CreateCycleCountDetailRequest>? CycleCountDetails { get; set; }
    }

    public class CreateCycleCountDetailRequest
    {
        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Bin is required.")]
        public int BinId { get; set; }

        [Required(ErrorMessage = "System Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "System Quantity must be 0 or greater.")]
        public int SystemQty { get; set; }

        [Required(ErrorMessage = "Actual Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Actual Quantity must be 0 or greater.")]
        public int ActualQty { get; set; }

        public string? Remarks { get; set; }
    }
}