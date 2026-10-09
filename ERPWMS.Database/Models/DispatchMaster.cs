using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class DispatchMaster
{
    public int DispatchId { get; set; }

    public int SalesId { get; set; }

    public int WarehouseId { get; set; }

    public string DispatchNo { get; set; } = null!;

    public DateTime? DispatchDate { get; set; }

    public string CarrierName { get; set; } = null!;

    public string? TrackingNo { get; set; }

    public string? Status { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public int? CurrentStageId { get; set; }

    public string? DispatchedBy { get; set; }

    public DateTime? DeliveryDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual WorkflowStage? CurrentStage { get; set; }

    public virtual ICollection<DeliveryChallan> DeliveryChallans { get; set; } = new List<DeliveryChallan>();

    public virtual ICollection<DispatchDetail> DispatchDetails { get; set; } = new List<DispatchDetail>();

    public virtual User? DispatchedByNavigation { get; set; }

    public virtual SalesMaster Sales { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
}
