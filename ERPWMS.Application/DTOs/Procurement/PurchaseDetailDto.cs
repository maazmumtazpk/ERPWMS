using System;

namespace ERPWMS.Application.DTOs.Procurement
{
    public class PurchaseDetailDto
    {
        public int DetailId { get; set; }
        public int PurchaseId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? DiscountPercent { get; set; }
        public string? BatchNo { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public int? ReceivedQty { get; set; }
        public int? RemainingQty => Quantity - (ReceivedQty ?? 0);
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}