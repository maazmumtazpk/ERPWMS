using MediatR;
using ERPWMS.Application.DTOs.Product;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Product
{
    public class GetProductsQuery : IRequest<ProductsListDto>
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? BrandId { get; set; }
        public int? UnitId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsSerialized { get; set; }
        public bool? IsBatchManaged { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool IncludeDeleted { get; set; } = false;
        public string? SortBy { get; set; }
        public bool SortDescending { get; set; } = false;
    }
}
