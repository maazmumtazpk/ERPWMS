using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Branch
{
    public int BranchId { get; set; }

    public int CompanyId { get; set; }

    public string BranchName { get; set; } = null!;

    public string? BranchCode { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

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

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
}
