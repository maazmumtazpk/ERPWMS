using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Brand
{
    public int BrandId { get; set; }

    public string BrandName { get; set; } = null!;

    public string? BrandCode { get; set; }

    public string? LogoUrl { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? DeletedByName { get; set; }

    public string? CreatedByName { get; set; }

    public string? UpdatedByName { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
