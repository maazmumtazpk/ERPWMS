using System;

namespace ERPWMS.Application.DTOs.ProductImage
{
    public class ProductImageDto
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string ImageURL { get; set; } = null!;
        public bool? IsPrimary { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
