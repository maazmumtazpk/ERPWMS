using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Procurement
{
    public class PurchaseReturnDto
    {
        public int ReturnId { get; set; }
        public int PurchaseId { get; set; }
        public string PurchaseOrderNo { get; set; } = string.Empty;
        public int? GrnId { get; set; }
        public string GrnNo { get; set; } = string.Empty;
        public DateTime? ReturnDate { get; set; }
        public string ReturnNo { get; set; } = string.Empty;
        public string ReturnReason { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }
        public int? WorkflowInstanceId { get; set; }
        public int? CurrentStageId { get; set; }
        public string? CurrentStageName { get; set; }
        public string? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public List<PurchaseReturnDetailDto>? PurchaseReturnDetails { get; set; }
    }
}


