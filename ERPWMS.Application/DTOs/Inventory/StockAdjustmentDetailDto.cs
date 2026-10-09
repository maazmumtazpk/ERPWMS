using System;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class StockAdjustmentDetailDto
    {
        public int AdjustmentDetailId { get; set; }
        public int AdjustmentId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int BinId { get; set; }
        public string BinName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int PreviousQty { get; set; }
        public int NewQty { get; set; }
        public string? BatchNo { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}