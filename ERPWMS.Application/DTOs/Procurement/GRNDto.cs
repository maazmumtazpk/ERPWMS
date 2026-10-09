using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Procurement
{
    public class GrnDto
    {
        public int GrnId { get; set; }
        public int PurchaseId { get; set; }
        public string PurchaseOrderNo { get; set; } = string.Empty;
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string GrnNo { get; set; } = string.Empty;
        public DateTime? ReceiptDate { get; set; }
        public int TotalReceivedQty { get; set; }
        public int TotalAcceptedQty { get; set; }
        public int? TotalRejectedQty { get; set; }
        public string? Status { get; set; }
        public int? WorkflowInstanceId { get; set; }
        public int? CurrentStageId { get; set; }
        public string? CurrentStageName { get; set; }
        public string CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public List<GrndetailDto>? Grndetails { get; set; }
    }
}