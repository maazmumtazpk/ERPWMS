using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class BankAccount
{
    public int BankAccountId { get; set; }

    public int CompanyId { get; set; }

    public string AccountTitle { get; set; } = null!;

    public string AccountNo { get; set; } = null!;

    public string? Iban { get; set; }

    public string BankName { get; set; } = null!;

    public string? BranchCode { get; set; }

    public string? AccountType { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public decimal OpeningBalance { get; set; }

    public virtual ICollection<BankReconciliation> BankReconciliations { get; set; } = new List<BankReconciliation>();

    public virtual ICollection<BankTransaction> BankTransactions { get; set; } = new List<BankTransaction>();

    public virtual Company Company { get; set; } = null!;

    public virtual ICollection<PaymentVoucher> PaymentVouchers { get; set; } = new List<PaymentVoucher>();

    public virtual ICollection<ReceiptVoucher> ReceiptVouchers { get; set; } = new List<ReceiptVoucher>();
}
