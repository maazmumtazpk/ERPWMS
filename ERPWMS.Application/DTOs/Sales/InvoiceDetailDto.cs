using System;

namespace ERPWMS.Application.DTOs.Sales
{
    public class InvoiceDetailDto
    {
        public int InvoiceDetailId { get; set; }
        public int InvoiceId { get; set; }
        public int SalesDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public decimal? TaxAmount { get; set; }
        public string? BatchNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}