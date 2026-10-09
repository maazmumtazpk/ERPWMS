using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class DeliveryChallan
{
    public int ChallanId { get; set; }

    public int SalesId { get; set; }

    public int DispatchId { get; set; }

    public string ChallanNo { get; set; } = null!;

    public DateTime? ChallanDate { get; set; }

    public string? Status { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public int? CurrentStageId { get; set; }

    public string? ReceivedBy { get; set; }

    public DateTime? ReceivedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual WorkflowStage? CurrentStage { get; set; }

    public virtual DispatchMaster Dispatch { get; set; } = null!;

    public virtual SalesMaster Sales { get; set; } = null!;

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
}
