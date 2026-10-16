using System;
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.Product
{
    public class ProductDto
    {
        // ============================
        // Primary Key
        // ============================

        public int ProductId { get; set; }


        // ============================
        // Category / Classification
        // ============================

        [Required(ErrorMessage = "Category is required.")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Category")]
        public string? CategoryName { get; set; }

        [Display(Name = "Sub Category")]
        public int? SubCategoryId { get; set; }

        [Display(Name = "Sub Category")]
        public string? SubCategoryName { get; set; }

        [Display(Name = "Brand")]
        public int? BrandId { get; set; }

        [Display(Name = "Brand")]
        public string? BrandName { get; set; }

        [Required(ErrorMessage = "Unit is required.")]
        [Display(Name = "Unit")]
        public int UnitId { get; set; }

        [Display(Name = "Unit")]
        public string? UnitName { get; set; }


        // ============================
        // Product Information
        // ============================

        [Required(ErrorMessage = "Product name is required.")]
        [StringLength(
            200,
            MinimumLength = 2,
            ErrorMessage = "Product name must be between 2 and 200 characters.")]
        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = string.Empty;

        [StringLength(100)]
        [Display(Name = "SKU")]
        public string? SKU { get; set; }

        [StringLength(100)]
        [Display(Name = "Barcode")]
        public string? Barcode { get; set; }

        [StringLength(100)]
        [Display(Name = "QR Code")]
        public string? QRCode { get; set; }

        [StringLength(1000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }


        // ============================
        // Dimensions / Weight
        // ============================

        [Range(0, double.MaxValue)]
        [Display(Name = "Weight")]
        public decimal? Weight { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Length")]
        public decimal? Length { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Width")]
        public decimal? Width { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Height")]
        public decimal? Height { get; set; }


        // ============================
        // Stock Levels
        // ============================

        [Range(0, double.MaxValue)]
        [Display(Name = "Minimum Stock Level")]
        public decimal? MinStockLevel { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Maximum Stock Level")]
        public decimal? MaxStockLevel { get; set; }

        [Range(0, double.MaxValue)]
        [Display(Name = "Reorder Level")]
        public decimal? ReorderLevel { get; set; }


        // ============================
        // Product Management
        // ============================

        [Display(Name = "Serialized Product")]
        public bool IsSerialized { get; set; }

        [Display(Name = "Batch Managed")]
        public bool IsBatchManaged { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }


        // ============================
        // Soft Delete
        // ============================

        public bool IsDeleted { get; set; }

        public string? DeletedBy { get; set; }

        public DateTime? DeletedDate { get; set; }


        // ============================
        // Audit
        // ============================

        public string? CreatedBy { get; set; }

        public DateTime? CreatedDate { get; set; }

        public string? UpdatedBy{ get; set; }

        public DateTime? UpdatedDate { get; set; }


        // ============================
        // Related Data Summary
        // ============================

        public int AttributeCount { get; set; }

        public int ImageCount { get; set; }

        public string? PrimaryImageUrl { get; set; }
    }
}
