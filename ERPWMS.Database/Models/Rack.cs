using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Rack
{
    public int RackId { get; set; }

    public int ZoneId { get; set; }

    public string RackName { get; set; } = null!;

    public string? RackCode { get; set; }

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

    public virtual ICollection<Bin> Bins { get; set; } = new List<Bin>();

    public virtual Zone Zone { get; set; } = null!;

    [NotMapped]
    public virtual User? CreatedByNavigation { get; set; }

    [NotMapped]
    public virtual User? UpdatedByNavigation { get; set; }
}
