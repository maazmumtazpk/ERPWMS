using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Company
{
    public int CompanyId { get; set; }

    public string CompanyName { get; set; } = null!;

    public string? RegistrationNo { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

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

    public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

    public virtual ICollection<Branch> Branches { get; set; } = new List<Branch>();

    public virtual ICollection<ChartOfAccount> ChartOfAccounts { get; set; } = new List<ChartOfAccount>();

    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    public virtual ICollection<TaxSetting> TaxSettings { get; set; } = new List<TaxSetting>();
}
