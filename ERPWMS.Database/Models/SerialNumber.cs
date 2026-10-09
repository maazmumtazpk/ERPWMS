using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class SerialNumber
{
    public int SerialId { get; set; }

    public int ProductId { get; set; }

    public int GrndetailId { get; set; }

    public string SerialNo { get; set; } = null!;

    public string? Status { get; set; }

    public int? SalesDetailId { get; set; }

    public int? CustomerId { get; set; }

    public DateTime? OutDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Grndetail Grndetail { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual SalesDetail? SalesDetail { get; set; }
}
