using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public int SalesId { get; set; }

    public string InvoiceNo { get; set; } = null!;

    public DateTime? InvoiceDate { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? Discount { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal NetAmount { get; set; }

    public decimal? PaidAmount { get; set; }

    public decimal? BalanceDue { get; set; }

    public string? Status { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public int? CurrentStageId { get; set; }

    public DateOnly DueDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual WorkflowStage? CurrentStage { get; set; }

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

    public virtual SalesMaster Sales { get; set; } = null!;

    public virtual ICollection<SalesReturn> SalesReturns { get; set; } = new List<SalesReturn>();

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
    public bool? IsActive { get; set; }
}
