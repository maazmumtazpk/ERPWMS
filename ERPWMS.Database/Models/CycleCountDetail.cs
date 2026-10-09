using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class CycleCountDetail
{
    public int CountDetailId { get; set; }

    public int CountId { get; set; }

    public int ProductId { get; set; }

    public int BinId { get; set; }

    public int SystemQty { get; set; }

    public int ActualQty { get; set; }

    public int Variance { get; set; }

    public string? Remarks { get; set; }

    public bool? IsDeleted { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Bin Bin { get; set; } = null!;

    public virtual CycleCount Count { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
