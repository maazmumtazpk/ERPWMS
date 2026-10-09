using System;

namespace ERPWMS.Application.DTOs.Supplier
{
    public class SupplierListDto
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;
        public string? SupplierCode { get; set; }
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
