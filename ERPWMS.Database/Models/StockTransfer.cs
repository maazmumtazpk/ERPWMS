using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class StockTransfer
{
    public int TransferId { get; set; }

    public int FromWarehouseId { get; set; }

    public int ToWarehouseId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public DateTime? TransferDate { get; set; }

    public string TransferNo { get; set; } = null!;

    public string? Status { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public int? CurrentStageId { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public string? ReceivedBy { get; set; }

    public DateTime? ReceivedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual WorkflowStage? CurrentStage { get; set; }

    public virtual Warehouse FromWarehouse { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual User? ReceivedByNavigation { get; set; }

    public virtual ICollection<StockTransferDetail> StockTransferDetails { get; set; } = new List<StockTransferDetail>();

    public virtual Warehouse ToWarehouse { get; set; } = null!;

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
    public bool? IsActive { get; set; }
}
