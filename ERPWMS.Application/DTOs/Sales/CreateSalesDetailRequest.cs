using System;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Sales
{
    public class CreateSalesDetailRequest
    {
        public int DetailId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Unit Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Unit Price must be greater than 0.")]
        public decimal UnitPrice { get; set; }

        [Range(0, 100, ErrorMessage = "Discount percent must be between 0 and 100.")]
        public decimal? DiscountPercent { get; set; }

        [StringLength(50)]
        public string? BatchNo { get; set; }
    }
}
