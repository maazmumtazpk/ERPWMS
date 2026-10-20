using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Supplier
{
    public int SupplierId { get; set; }

    public string SupplierName { get; set; } = null!;

    public string? SupplierCode { get; set; }

    public string? ContactPerson { get; set; }

    public string Phone { get; set; } = null!;

    public string? Mobile { get; set; }

    public string? Email { get; set; }

    public string? Website { get; set; }

    public string Address { get; set; } = null!;

    public string? City { get; set; }

    public string? State { get; set; }

    public string? Country { get; set; }

    public string? PostalCode { get; set; }

    public string? TaxId { get; set; }

    public string? NTN { get; set; }

    public string? BankName { get; set; }

    public string? BankAccountNo { get; set; }

    public string? BankBranch { get; set; }

    public decimal? CreditLimit { get; set; }

    public string? PaymentTerms { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }




    public virtual ICollection<PaymentVoucher> PaymentVouchers { get; set; } = new List<PaymentVoucher>();

    public virtual ICollection<PurchaseMaster> PurchaseMasters { get; set; } = new List<PurchaseMaster>();

    public virtual ICollection<SupplierContact> SupplierContacts { get; set; } = new List<SupplierContact>();

    public virtual ICollection<SupplierQuotation> SupplierQuotations { get; set; } = new List<SupplierQuotation>();

    [NotMapped]
    public virtual User? CreatedByNavigation { get; set; }

    [NotMapped]
    public virtual User? UpdatedByNavigation { get; set; }
}

