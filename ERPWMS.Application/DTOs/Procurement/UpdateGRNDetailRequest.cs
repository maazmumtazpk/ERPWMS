using System;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Procurement
{
    public class UpdateGrndetailRequest
    {
        public int GrndetailId { get; set; }

        [Required(ErrorMessage = "Purchase Detail is required.")]
        public int PurchaseDetailId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Bin is required.")]
        public int BinId { get; set; }

        [Required(ErrorMessage = "Received Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Received quantity must be 0 or greater.")]
        public int ReceivedQty { get; set; }

        [Required(ErrorMessage = "Accepted Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Accepted quantity must be 0 or greater.")]
        public int AcceptedQty { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Rejected quantity must be 0 or greater.")]
        public int? RejectedQty { get; set; }

        [StringLength(500)]
        public string? RejectionReason { get; set; }

        public string? BatchNo { get; set; }

        public DateOnly? ExpiryDate { get; set; }

        public string? SerialNumbers { get; set; }
    }
}