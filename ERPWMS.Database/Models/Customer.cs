using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string CustomerName { get; set; } = null!;

    public string? CustomerCode { get; set; }

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

    public decimal? CreditLimit { get; set; }

    public int? CreditDays { get; set; }

    public string? PaymentTerms { get; set; }

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }




    public virtual ICollection<CustomerComplaint> CustomerComplaints { get; set; } = new List<CustomerComplaint>();

    public virtual ICollection<CustomerContact> CustomerContacts { get; set; } = new List<CustomerContact>();

    public virtual ICollection<CustomerFeedback> CustomerFeedbacks { get; set; } = new List<CustomerFeedback>();

    public virtual ICollection<CustomerInquiry> CustomerInquiries { get; set; } = new List<CustomerInquiry>();

    public virtual ICollection<ReceiptVoucher> ReceiptVouchers { get; set; } = new List<ReceiptVoucher>();

    public virtual ICollection<SalesMaster> SalesMasters { get; set; } = new List<SalesMaster>();

    public virtual ICollection<SerialNumber> SerialNumbers { get; set; } = new List<SerialNumber>();

    [NotMapped]
    public virtual User? CreatedByNavigation { get; set; }

    [NotMapped]
    public virtual User? UpdatedByNavigation { get; set; }
}

