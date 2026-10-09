using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class SupplierQuotation
{
    public int QuotationId { get; set; }

    public int SupplierId { get; set; }

    public string QuotationNo { get; set; } = null!;

    public DateTime? QuotationDate { get; set; }

    public DateOnly? ValidUntil { get; set; }

    public decimal TotalAmount { get; set; }

    public decimal? Discount { get; set; }

    public decimal? TaxAmount { get; set; }

    public decimal NetAmount { get; set; }

    public string? Status { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
}
