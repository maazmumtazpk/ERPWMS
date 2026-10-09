namespace ERPWMS.Application.DTOs.Product
{
    public class ProductFilterDto
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? UnitId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsSerialized { get; set; }
        public bool? IsBatchManaged { get; set; }
        public bool? LowStock { get; set; }
        public bool? OutOfStock { get; set; }
    }
}
