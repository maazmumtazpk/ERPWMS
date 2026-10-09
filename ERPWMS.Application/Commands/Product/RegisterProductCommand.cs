using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Product
{
    public class RegisterProductCommand : IRequest<int>
    {
        public int CategoryId { get; set; }

        public int SubCategoryId { get; set; }

        public int BrandId { get; set; }

        public int UnitId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public string SKU { get; set; } = string.Empty;

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

        public bool IsSerialized { get; set; }

        public bool IsBatchManaged { get; set; }

        public bool IsActive { get; set; } = true;

        public string CreatedBy { get; set; }
    }
}