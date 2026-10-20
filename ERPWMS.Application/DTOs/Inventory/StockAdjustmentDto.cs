using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class StockAdjustmentDto
    {
        public int AdjustmentId { get; set; }
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int BinId { get; set; }
        public string BinName { get; set; } = string.Empty;
        public string AdjustmentType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int PreviousQty { get; set; }
        public int NewQty { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Status { get; set; }
        public int? WorkflowInstanceId { get; set; }
        public int? CurrentStageId { get; set; }
        public string? CurrentStageName { get; set; }
        public string? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public List<StockAdjustmentDetailDto>? AdjustmentDetails { get; set; }
    }
}

