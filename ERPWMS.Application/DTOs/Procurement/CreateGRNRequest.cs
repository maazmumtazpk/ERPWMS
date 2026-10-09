using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Procurement
{
    public class CreateGrnRequest
    {
        [Required(ErrorMessage = "Purchase Order is required.")]
        public int PurchaseId { get; set; }

        [Required(ErrorMessage = "Warehouse is required.")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Grn Number is required.")]
        [StringLength(50)]
        public string GrnNo { get; set; } = string.Empty;

        public DateTime? ReceiptDate { get; set; }

        public string? Remarks { get; set; }

        [Required(ErrorMessage = "At least one product is required.")]
        [MinLength(1, ErrorMessage = "At least one product is required.")]
        public List<CreateGrndetailRequest> Grndetails { get; set; } = new();
    }
}