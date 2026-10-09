using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class CycleCount
{
    public int CycleCountId { get; set; }

    public int WarehouseId { get; set; }

    public int BinId { get; set; }

    public int ProductId { get; set; }

    public int SystemQty { get; set; }

    public int ActualQty { get; set; }

    public int Variance { get; set; }

    public string? CountedBy { get; set; }

    public DateTime? CountedDate { get; set; }

    public string? VerifiedBy { get; set; }

    public DateTime? VerifiedDate { get; set; }

    public string? Status { get; set; }

    public string? Remarks { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Bin Bin { get; set; } = null!;

    public virtual User? CountedByNavigation { get; set; }

    public virtual ICollection<CycleCountDetail> CycleCountDetails { get; set; } = new List<CycleCountDetail>();

    public virtual Product Product { get; set; } = null!;

    public virtual User? VerifiedByNavigation { get; set; }

    public virtual Warehouse Warehouse { get; set; } = null!;
    public bool? IsActive { get; set; }
}
