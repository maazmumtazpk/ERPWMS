using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Unit
{
    public int UnitId { get; set; }

    public string UnitName { get; set; } = null!;

    public string? UnitCode { get; set; }

    public bool? IsBaseUnit { get; set; }

    public decimal? ConversionFactor { get; set; }

    public int? BaseUnitId { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }




    public virtual Unit? BaseUnit { get; set; }

    public virtual ICollection<Unit> InverseBaseUnit { get; set; } = new List<Unit>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}

