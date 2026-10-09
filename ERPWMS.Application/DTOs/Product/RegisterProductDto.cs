namespace ERPWMS.Application.DTOs.Product
{
    public class RegisterProductDto
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

        // IMPORTANT:
        // Checkbox properties must be bool, not bool?
        public bool IsSerialized { get; set; } = false;

        public bool IsBatchManaged { get; set; } = false;

        public bool IsActive { get; set; } = true;
    }
}