using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Sales
{
    public class InvoiceDto
    {
        public int InvoiceId { get; set; }
        public int SalesId { get; set; }
        public string SalesOrderNo { get; set; } = string.Empty;
        public string InvoiceNo { get; set; } = string.Empty;
        public DateTime? InvoiceDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? Discount { get; set; }
        public decimal? TaxAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal? PaidAmount { get; set; }
        public decimal BalanceDue { get; set; }
        public string? Status { get; set; }
        public DateOnly DueDate { get; set; }
        public int? WorkflowInstanceId { get; set; }
        public int? CurrentStageId { get; set; }
        public string? CurrentStageName { get; set; }
        public string CreatedBy { get; set; }
        public string? CreatedByName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
        public List<InvoiceDetailDto>? InvoiceDetails { get; set; }
    }
}