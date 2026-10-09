using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class BankReconciliation
{
    public int ReconciliationId { get; set; }

    public int BankAccountId { get; set; }

    public DateOnly StatementDate { get; set; }

    public decimal StatementBalance { get; set; }

    public decimal SystemBalance { get; set; }

    public decimal Difference { get; set; }

    public string? Status { get; set; }

    public DateTime? ReconciliationDate { get; set; }

    public string? Remarks { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual BankAccount BankAccount { get; set; } = null!;
}
