using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class ChartOfAccount
{
    public int AccountId { get; set; }

    public int? ParentAccountId { get; set; }

    public string AccountCode { get; set; } = null!;

    public string AccountName { get; set; } = null!;

    public string AccountType { get; set; } = null!;

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? CompanyId { get; set; }

    public virtual ICollection<AccountLedger> AccountLedgers { get; set; } = new List<AccountLedger>();

    public virtual Company? Company { get; set; }

    public virtual ICollection<ChartOfAccount> InverseParentAccount { get; set; } = new List<ChartOfAccount>();

    public virtual ICollection<JournalVoucherDetail> JournalVoucherDetails { get; set; } = new List<JournalVoucherDetail>();

    public virtual ChartOfAccount? ParentAccount { get; set; }

    public virtual ICollection<PaymentVoucherDetail> PaymentVoucherDetails { get; set; } = new List<PaymentVoucherDetail>();

    public virtual ICollection<ReceiptVoucherDetail> ReceiptVoucherDetails { get; set; } = new List<ReceiptVoucherDetail>();
}
