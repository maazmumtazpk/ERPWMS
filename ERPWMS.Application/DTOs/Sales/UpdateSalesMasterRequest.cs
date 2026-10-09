using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Sales
{
    public class UpdateSalesMasterRequest
    {
        public int SalesId { get; set; }

        [Required(ErrorMessage = "Customer is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Warehouse is required.")]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "Sales Order Number is required.")]
        [StringLength(50)]
        public string SalesOrderNo { get; set; } = string.Empty;

        public DateTime? OrderDate { get; set; }

        public DateOnly? DeliveryDate { get; set; }

        public decimal? Discount { get; set; }

        public decimal? TaxAmount { get; set; }

        public string? Remarks { get; set; }

        public string? Status { get; set; }

        public List<CreateSalesDetailRequest> SalesDetails { get; set; } = new();
    }
}