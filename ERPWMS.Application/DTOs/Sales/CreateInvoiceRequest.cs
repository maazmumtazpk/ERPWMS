using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Sales
{
    public class CreateInvoiceRequest
    {
        [Required(ErrorMessage = "Sales Order is required.")]
        public int SalesId { get; set; }

        [Required(ErrorMessage = "Invoice Number is required.")]
        [StringLength(50)]
        public string InvoiceNo { get; set; } = string.Empty;

        public DateTime? InvoiceDate { get; set; }

        public decimal? Discount { get; set; }

        public decimal? TaxAmount { get; set; }

        [Required(ErrorMessage = "Due Date is required.")]
        public DateOnly DueDate { get; set; }

        public string? Remarks { get; set; }

        [Required(ErrorMessage = "At least one item is required.")]
        [MinLength(1, ErrorMessage = "At least one item is required.")]
        public List<CreateInvoiceDetailRequest> InvoiceDetails { get; set; } = new();
    }
}
