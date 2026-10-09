using System;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class StockLedgerFilterDto
    {
        public int? ProductId { get; set; }
        public int? WarehouseId { get; set; }
        public int? BinId { get; set; }
        public string? TransactionType { get; set; }
        public string? ReferenceDocNo { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string? BatchNo { get; set; }
    }
}