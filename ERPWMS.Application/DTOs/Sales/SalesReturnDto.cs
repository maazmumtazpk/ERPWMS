using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Sales
{
    public class SalesReturnDto
    {
        public int ReturnId { get; set; }
        public int SalesId { get; set; }
        public string? SalesOrderNo { get; set; }

        public int? InvoiceId { get; set; }
        public string? InvoiceNo { get; set; }

        public DateTime? ReturnDate { get; set; }
        public string? ReturnNo { get; set; }
        public string? ReturnReason { get; set; }

        public decimal TotalAmount { get; set; }
        public string? Status { get; set; }

        // ✅ FIX: ApprovedBy string? — Model ke mutabiq
        public string? ApprovedBy { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public int? WorkflowInstanceId { get; set; }
        public int? CurrentStageId { get; set; }
        public string? CurrentStageName { get; set; }

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public bool IsDeleted { get; set; }

        public List<SalesReturnDetailDto>? SalesReturnDetails { get; set; }
    }
}
