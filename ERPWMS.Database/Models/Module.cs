using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Module
{
    public int ModuleId { get; set; }

    public string ModuleName { get; set; } = null!;

    public string ModuleCode { get; set; } = null!;

    public string? Description { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    public virtual ICollection<ModulePermission> ModulePermissions { get; set; } = new List<ModulePermission>();
}
