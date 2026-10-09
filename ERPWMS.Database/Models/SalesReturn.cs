using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class SalesReturn
{
    public int ReturnId { get; set; }

    public int SalesId { get; set; }

    public int? InvoiceId { get; set; }

    public DateTime? ReturnDate { get; set; }

    public string ReturnNo { get; set; } = null!;

    public string ReturnReason { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string? Status { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public int? CurrentStageId { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual WorkflowStage? CurrentStage { get; set; }

    public virtual Invoice? Invoice { get; set; }

    public virtual SalesMaster Sales { get; set; } = null!;

    public virtual ICollection<SalesReturnDetail> SalesReturnDetails { get; set; } = new List<SalesReturnDetail>();

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual User? CreatedByNavigation { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual User? UpdatedByNavigation { get; set; }
}
