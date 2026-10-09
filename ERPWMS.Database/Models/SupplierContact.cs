using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class SupplierContact
{
    public int ContactId { get; set; }

    public int SupplierId { get; set; }

    public string ContactName { get; set; } = null!;

    public string? Phone { get; set; }

    public string Mobile { get; set; } = null!;

    public string? Email { get; set; }

    public string? Department { get; set; }

    public bool? IsPrimary { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;

    [NotMapped]
    public virtual User? CreatedByNavigation { get; set; }

    [NotMapped]
    public virtual User? UpdatedByNavigation { get; set; }
}
