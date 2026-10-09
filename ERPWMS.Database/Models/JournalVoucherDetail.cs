using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class JournalVoucherDetail
{
    public int DetailId { get; set; }

    public int VoucherId { get; set; }

    public int AccountId { get; set; }

    public decimal Debit { get; set; }

    public decimal Credit { get; set; }

    public string? Narration { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual ChartOfAccount Account { get; set; } = null!;

    public virtual JournalVoucher Voucher { get; set; } = null!;
}
