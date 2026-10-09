using System;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Sales
{
    public class CreateSalesReturnDetailRequest
    {
        [Required(ErrorMessage = "Invoice Detail is required.")]
        public int InvoiceDetailId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0.")]
        public decimal UnitPrice { get; set; }

        public string? BatchNo { get; set; }

        public string? Remarks { get; set; }
    }
}