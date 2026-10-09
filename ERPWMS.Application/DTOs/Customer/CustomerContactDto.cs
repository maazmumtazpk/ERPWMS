using System;

namespace ERPWMS.Application.DTOs.Customer
{
    public class CustomerContactDto
    {
        public int ContactId { get; set; }
        public int CustomerId { get; set; }
        public string ContactName { get; set; } = null!;
        public string? Phone { get; set; }
        public string Mobile { get; set; } = null!;
        public string? Email { get; set; }
        public string? Department { get; set; }
        public bool? IsPrimary { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CustomerName { get; set; }
    }
}
