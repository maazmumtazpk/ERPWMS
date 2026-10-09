using System;

namespace ERPWMS.Application.DTOs.Supplier
{
    public class SupplierContactDto
    {
        public int ContactId { get; set; }
        public int SupplierId { get; set; }
        public string ContactName { get; set; } = null!;
        public string? Phone { get; set; }
        public string Mobile { get; set; } = null!;
        public string? Email { get; set; }
        public string? Department { get; set; }
        public bool? IsPrimary { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? SupplierName { get; set; }
    }
}
