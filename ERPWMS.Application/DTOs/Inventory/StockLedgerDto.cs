using System;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class StockLedgerDto
    {
        public int LedgerId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int BinId { get; set; }
        public string BinName { get; set; } = string.Empty;
        public string TransactionType { get; set; } = string.Empty;
        public string ReferenceDocNo { get; set; } = string.Empty;
        public string ReferenceDocType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int Balance { get; set; }
        public decimal UnitCost { get; set; }
        public string? BatchNo { get; set; }
        public DateOnly? ExpiryDate { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
