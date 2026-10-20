using System;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class BatchTrackingDto
    {
        public int BatchId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int GrndetailId { get; set; }
        public string BatchNo { get; set; } = string.Empty;
        public DateOnly ExpiryDate { get; set; }
        public int Quantity { get; set; }
        public int RemainingQty { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
        public bool IsExpired => ExpiryDate < DateOnly.FromDateTime(DateTime.UtcNow);
        public bool IsNearExpiry => ExpiryDate > DateOnly.FromDateTime(DateTime.UtcNow) &&
                                    ExpiryDate <= DateOnly.FromDateTime(DateTime.UtcNow.AddDays(30));
        public string StatusDisplay => IsExpired ? "🔴 Expired" :
                                        IsNearExpiry ? "🟡 Near Expiry" :
                                        (IsActive == true ? "✅ Active" : "⛔ Inactive");
        public int AvailableQty => RemainingQty;
    }
}
