using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Zone
{
    public int ZoneId { get; set; }

    public int WarehouseId { get; set; }

    public string ZoneName { get; set; } = null!;

    public string? ZoneCode { get; set; }

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

    public virtual ICollection<Rack> Racks { get; set; } = new List<Rack>();

    public virtual Warehouse Warehouse { get; set; } = null!;

    [NotMapped]
    public virtual User? CreatedByNavigation { get; set; }

    [NotMapped]
    public virtual User? UpdatedByNavigation { get; set; }
}
