using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Product
{
    public class ModifyProductCommand : IRequest<bool>
    {
        // ============================
        // Primary Key
        // ============================

        public int ProductId { get; set; }


        // ============================
        // Category / Classification
        // ============================

        public int CategoryId { get; set; }

        public int? SubCategoryId { get; set; }

        public int? BrandId { get; set; }

        public int UnitId { get; set; }


        // ============================
        // Basic Product Information
        // ============================

        public string ProductName { get; set; } = string.Empty;

        public string? SKU { get; set; }

        public string? Barcode { get; set; }

        public string? QRCode { get; set; }

        public string? Description { get; set; }


        // ============================
        // Dimensions / Weight
        // ============================

        public decimal? Weight { get; set; }

        public decimal? Length { get; set; }

        public decimal? Width { get; set; }

        public decimal? Height { get; set; }


        // ============================
        // Stock Levels
        // ============================

        public decimal? MinStockLevel { get; set; }

        public decimal? MaxStockLevel { get; set; }

        public decimal? ReorderLevel { get; set; }


        // ============================
        // Product Management
        // ============================

        public bool IsSerialized { get; set; }

        public bool IsBatchManaged { get; set; }

        public bool IsActive { get; set; }


        // ============================
        // Audit
        // ============================

        public int UpdatedBy { get; set; }
    }
}