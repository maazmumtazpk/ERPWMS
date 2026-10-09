using System;

namespace ERPWMS.Application.DTOs.Supplier
{
    public class RegisterSupplierRequest
    {
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
    }
}
