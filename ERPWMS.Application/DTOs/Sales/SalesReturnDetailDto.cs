using System;

namespace ERPWMS.Application.DTOs.Sales
{
    public class SalesReturnDetailDto
    {
        public int ReturnDetailId { get; set; }
        public int ReturnId { get; set; }
        public int InvoiceDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? BatchNo { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}