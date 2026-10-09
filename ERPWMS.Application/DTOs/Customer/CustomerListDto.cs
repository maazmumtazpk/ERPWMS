using System;

namespace ERPWMS.Application.DTOs.Customer
{
    public class CustomerListDto
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;
        public string? CustomerCode { get; set; }
        public string? ContactPerson { get; set; }
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? NTN { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int ContactCount { get; set; }
    }
}
