using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class ReceiptVoucher
{
    public int ReceiptId { get; set; }

    public int? SalesId { get; set; }

    public int? CustomerId { get; set; }

    public int BankAccountId { get; set; }

    public string ReceiptNo { get; set; } = null!;

    public DateTime? ReceiptDate { get; set; }

    public decimal Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public string? ChequeNo { get; set; }

    public string? ReferenceNo { get; set; }

    public string? Description { get; set; }

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

    public virtual BankAccount BankAccount { get; set; } = null!;

    public virtual WorkflowStage? CurrentStage { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<ReceiptVoucherDetail> ReceiptVoucherDetails { get; set; } = new List<ReceiptVoucherDetail>();

    public virtual SalesMaster? Sales { get; set; }

    public virtual WorkflowInstance? WorkflowInstance { get; set; }
}
