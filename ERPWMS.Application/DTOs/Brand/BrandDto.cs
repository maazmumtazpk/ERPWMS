using System;

namespace ERPWMS.Application.DTOs.Brand
{
    public class BrandDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;
        public string? BrandCode { get; set; }
        public string? LogoURL { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string? DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int ProductCount { get; set; }
    }
}

