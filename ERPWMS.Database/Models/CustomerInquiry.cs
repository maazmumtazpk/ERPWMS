using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class CustomerInquiry
{
    public int InquiryId { get; set; }

    public int CustomerId { get; set; }

    public string? InquiryType { get; set; }

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? Status { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public int? CurrentStageId { get; set; }

    public int? AssignedTo { get; set; }

    public int? ResolvedBy { get; set; }

    public DateTime? ResolvedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual WorkflowStage? CurrentStage { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual User? ResolvedByNavigation { get; set; }

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
}
