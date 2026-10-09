using System;
using System.Collections.Generic;
using ERPWMS.Application.DTOs.ProductAttribute;
using ERPWMS.Application.DTOs.ProductImage;

namespace ERPWMS.Application.DTOs.Product
{
    public class ProductDetailDto
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string CategoryCode { get; set; } = null!;
        public int SubCategoryId { get; set; }
        public string SubCategoryName { get; set; } = null!;
        public string SubCategoryCode { get; set; } = null!;
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;
        public string BrandCode { get; set; } = null!;
        public string? BrandLogoURL { get; set; }
        public int UnitId { get; set; }
        public string UnitName { get; set; } = null!;
        public string UnitCode { get; set; } = null!;
        public string ProductName { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string? Barcode { get; set; }
        public string? QRCode { get; set; }
        public string? Description { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Length { get; set; }
        public decimal? Width { get; set; }
        public decimal? Height { get; set; }
        public int? MinStockLevel { get; set; }
        public int? MaxStockLevel { get; set; }
        public int? ReorderLevel { get; set; }
        public bool? IsSerialized { get; set; }
        public bool? IsBatchManaged { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string?DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedByName { get; set; } = null!;
        public DateTime? CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public string UpdatedByName { get; set; } = null!;
        public DateTime? UpdatedDate { get; set; }
        public List<ProductAttributeDto> Attributes { get; set; } = new List<ProductAttributeDto>();
        public List<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
        public string? PrimaryImageUrl { get; set; }
    }
}
