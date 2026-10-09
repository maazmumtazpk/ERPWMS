using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class BankTransaction
{
    public int TransactionId { get; set; }

    public int BankAccountId { get; set; }

    public DateTime? TransactionDate { get; set; }

    public decimal Amount { get; set; }

    public string? TransactionType { get; set; }

    public string? ReferenceNo { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual BankAccount BankAccount { get; set; } = null!;
}
