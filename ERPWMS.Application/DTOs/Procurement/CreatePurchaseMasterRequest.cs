using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Procurement
{
    public class CreatePurchaseMasterRequest
    {
        [Required(ErrorMessage = "Supplier is required.")]
        public int SupplierId { get; set; }

        [Required(ErrorMessage = "Warehouse is required.")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Purchase Order Number is required.")]
        [StringLength(50)]
        public string PurchaseOrderNo { get; set; } = string.Empty;

        public DateTime? OrderDate { get; set; }

        public DateOnly? ExpectedDeliveryDate { get; set; }

        public decimal? Discount { get; set; }

        public decimal? TaxAmount { get; set; }

        public string? Remarks { get; set; }

        [Required(ErrorMessage = "At least one product is required.")]
        [MinLength(1, ErrorMessage = "At least one product is required.")]
        public List<CreatePurchaseDetailRequest> PurchaseDetails { get; set; } = new();
    }
}