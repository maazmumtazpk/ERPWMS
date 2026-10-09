using System;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Sales
{
    public class UpdatePickingDetailRequest
    {
        public int PickingDetailId { get; set; }

        [Required(ErrorMessage = "Sales Detail is required.")]
        public int SalesDetailId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Bin is required.")]
        public int BinId { get; set; }

        [Required(ErrorMessage = "Picked Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Picked quantity must be 0 or greater.")]
        public int PickedQty { get; set; }

        public string? Status { get; set; }

        public string? BatchNo { get; set; }

        public string? SerialNumbers { get; set; }
    }
}