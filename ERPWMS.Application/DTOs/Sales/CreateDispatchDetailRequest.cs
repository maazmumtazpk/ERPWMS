using System;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Sales
{
    public class CreateDispatchDetailRequest
    {
        [Required(ErrorMessage = "Sales Detail is required.")]
        public int SalesDetailId { get; set; }

        [Required(ErrorMessage = "Product is required.")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }

        public string? BatchNo { get; set; }

        public string? SerialNumbers { get; set; }
    }
}