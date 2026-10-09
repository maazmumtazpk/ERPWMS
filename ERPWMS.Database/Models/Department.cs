using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Department
{
    public int DepartmentId { get; set; }

    public int CompanyId { get; set; }

    public int BranchId { get; set; }

    public string DepartmentName { get; set; } = null!;

    public string? DepartmentCode { get; set; }

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

    public virtual Branch Branch { get; set; } = null!;

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Designation> Designations { get; set; } = new List<Designation>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
