using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class ModulePermission
{
    public int PermissionId { get; set; }

    public int ModuleId { get; set; }

    public int RoleId { get; set; }

    public bool? CanView { get; set; }

    public bool? CanCreate { get; set; }

    public bool? CanEdit { get; set; }

    public bool? CanDelete { get; set; }

    public bool? CanApprove { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual Module Module { get; set; } = null!;

    public virtual AspNetRole Role { get; set; } = null!;
}
