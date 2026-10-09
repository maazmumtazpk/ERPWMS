using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class TaxSetting
{
    public int TaxId { get; set; }

    public string TaxName { get; set; } = null!;

    public string TaxCode { get; set; } = null!;

    public decimal TaxRate { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDefault { get; set; }

    public DateOnly EffectiveFrom { get; set; }

    public DateOnly? EffectiveTo { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int CompanyId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Company Company { get; set; } = null!;
}
