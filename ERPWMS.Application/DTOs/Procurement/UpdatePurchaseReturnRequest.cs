using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Procurement
{
    public class UpdatePurchaseReturnRequest
    {
        public int ReturnId { get; set; }

        [Required(ErrorMessage = "Purchase Order is required.")]
        public int PurchaseId { get; set; }

        public int? GrnId { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required(ErrorMessage = "Return Number is required.")]
        [StringLength(50)]
        public string ReturnNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Return Reason is required.")]
        [StringLength(500)]
        public string ReturnReason { get; set; } = string.Empty;

        public string? Status { get; set; }

        public List<CreatePurchaseReturnDetailRequest> PurchaseReturnDetails { get; set; } = new();
    }
}