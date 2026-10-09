using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class ProductAttribute
{
    public int AttributeId { get; set; }

    public int ProductId { get; set; }

    public string AttributeName { get; set; } = null!;

    public string AttributeValue { get; set; } = null!;

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Product Product { get; set; } = null!;
}
