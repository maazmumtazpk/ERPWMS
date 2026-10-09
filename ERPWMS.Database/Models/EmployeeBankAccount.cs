using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class EmployeeBankAccount
{
    public int BankAccountId { get; set; }

    public int EmployeeId { get; set; }

    public string BankName { get; set; } = null!;

    public string AccountNumber { get; set; } = null!;

    public string? Iban { get; set; }

    public string? AccountType { get; set; }

    public bool? IsPrimary { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
