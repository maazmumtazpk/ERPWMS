using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class AspNetRole
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string NormalizedName { get; set; } = null!;

    public string? ConcurrencyStamp { get; set; }

    public virtual ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; } = new List<AspNetRoleClaim>();

    public virtual ICollection<ModulePermission> ModulePermissions { get; set; } = new List<ModulePermission>();

    public virtual ICollection<WorkflowAssignment> WorkflowAssignments { get; set; } = new List<WorkflowAssignment>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
