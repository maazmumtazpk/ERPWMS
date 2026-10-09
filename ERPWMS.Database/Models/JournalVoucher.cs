using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class JournalVoucher
{
    public int VoucherId { get; set; }

    public string VoucherNo { get; set; } = null!;

    public DateTime? VoucherDate { get; set; }

    public decimal TotalDebit { get; set; }

    public decimal TotalCredit { get; set; }

    public string Description { get; set; } = null!;

    public string? Status { get; set; }

    public int? WorkflowInstanceId { get; set; }

    public int? CurrentStageId { get; set; }

    public string? ApprovedBy { get; set; }

    public DateTime? ApprovedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual User? ApprovedByNavigation { get; set; }

    public virtual WorkflowStage? CurrentStage { get; set; }

    public virtual ICollection<JournalVoucherDetail> JournalVoucherDetails { get; set; } = new List<JournalVoucherDetail>();

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
}
