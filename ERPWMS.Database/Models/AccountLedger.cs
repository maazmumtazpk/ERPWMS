using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class AccountLedger
{
    public int LedgerEntryId { get; set; }

    public int AccountId { get; set; }

    public int VoucherId { get; set; }

    public string VoucherType { get; set; } = null!;

    public DateTime? EntryDate { get; set; }

    public decimal? Debit { get; set; }

    public decimal? Credit { get; set; }

    public decimal Balance { get; set; }

    public string? Description { get; set; }

    public string? ReferenceNo { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ChartOfAccount Account { get; set; } = null!;
}
