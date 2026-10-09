using System;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Product
{
    public class ProductsListDto
    {
        public IEnumerable<ProductDto> Products { get; set; }
            = new List<ProductDto>();

        public int TotalCount { get; set; }

        public int CurrentPage { get; set; }

        public int PageSize { get; set; }

        public int TotalPages =>
            PageSize > 0
                ? (int)Math.Ceiling(
                    (double)TotalCount / PageSize)
                : 0;

        public string? SearchTerm { get; set; }

        public int? CategoryId { get; set; }

        public string? CategoryName { get; set; }

        public int? SubCategoryId { get; set; }

        public string? SubCategoryName { get; set; }

        public int? BrandId { get; set; }

        public string? BrandName { get; set; }

        public int? UnitId { get; set; }

        public string? UnitName { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsSerialized { get; set; }

        public bool? IsBatchManaged { get; set; }
    }
}