using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class PickingList
{
    public int PickingId { get; set; }

    public int SalesId { get; set; }

    public int WarehouseId { get; set; }

    public string PickingNo { get; set; } = null!;

    public DateTime? PickingDate { get; set; }

    public string? Status { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public int? CurrentStageId { get; set; }

    public int? AssignedTo { get; set; }

    public string? PickedBy { get; set; }

    public string? VerifiedBy { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User? AssignedToNavigation { get; set; }

    public virtual WorkflowStage? CurrentStage { get; set; }

    public virtual User? PickedByNavigation { get; set; }

    public virtual ICollection<PickingDetail> PickingDetails { get; set; } = new List<PickingDetail>();

    public virtual SalesMaster Sales { get; set; } = null!;

    public virtual User? VerifiedByNavigation { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual User? CreatedByNavigation { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual User? UpdatedByNavigation { get; set; }
}
