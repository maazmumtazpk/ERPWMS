using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class StockTransferDto
    {
        public int TransferId { get; set; }
        public int FromWarehouseId { get; set; }
        public string FromWarehouseName { get; set; } = string.Empty;
        public int ToWarehouseId { get; set; }
        public string ToWarehouseName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductCode { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime? TransferDate { get; set; }
        public string TransferNo { get; set; } = string.Empty;
        public string? Status { get; set; }
        public int? WorkflowInstanceId { get; set; }
        public int? CurrentStageId { get; set; }
        public string? CurrentStageName { get; set; }
        public string? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string? ReceivedBy { get; set; }
        public string? ReceivedByName { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public List<StockTransferDetailDto>? TransferDetails { get; set; }
    }
}

