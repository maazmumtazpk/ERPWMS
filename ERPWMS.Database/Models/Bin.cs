using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Bin
{
    public int BinId { get; set; }

    public int RackId { get; set; }

    public string BinName { get; set; } = null!;

    public string? BinCode { get; set; }

    public bool? IsOccupied { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }




    public virtual ICollection<CycleCountDetail> CycleCountDetails { get; set; } = new List<CycleCountDetail>();

    public virtual ICollection<CycleCount> CycleCounts { get; set; } = new List<CycleCount>();

    public virtual ICollection<Grndetail> Grndetails { get; set; } = new List<Grndetail>();

    public virtual ICollection<PickingDetail> PickingDetails { get; set; } = new List<PickingDetail>();

    public virtual Rack Rack { get; set; } = null!;

    public virtual ICollection<StockAdjustmentDetail> StockAdjustmentDetails { get; set; } = new List<StockAdjustmentDetail>();

    public virtual ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();

    public virtual ICollection<StockLedger> StockLedgers { get; set; } = new List<StockLedger>();

    public virtual ICollection<StockTransferDetail> StockTransferDetailFromBins { get; set; } = new List<StockTransferDetail>();

    public virtual ICollection<StockTransferDetail> StockTransferDetailToBins { get; set; } = new List<StockTransferDetail>();

    [NotMapped]
    public virtual User? CreatedByNavigation { get; set; }

    [NotMapped]
    public virtual User? UpdatedByNavigation { get; set; }
}

