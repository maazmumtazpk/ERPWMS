using System;

namespace ERPWMS.Application.DTOs.Customer
{
    public class RegisterCustomerRequest
    {
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
    }
}
