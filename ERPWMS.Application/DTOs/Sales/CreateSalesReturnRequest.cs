using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Sales
{
    public class CreateSalesReturnRequest
    {
        [Required(ErrorMessage = "Sales Order is required.")]
        public int SalesId { get; set; }

        [Required(ErrorMessage = "Invoice is required.")]
        public int InvoiceId { get; set; }

        public DateTime? ReturnDate { get; set; }

        [Required(ErrorMessage = "Return Number is required.")]
        [StringLength(50)]
        public string ReturnNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Return Reason is required.")]
        [StringLength(500)]
        public string ReturnReason { get; set; } = string.Empty;

        [Required(ErrorMessage = "At least one item is required.")]
        [MinLength(1, ErrorMessage = "At least one item is required.")]
        public List<CreateSalesReturnDetailRequest> SalesReturnDetails { get; set; } = new();
    }
}