using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.SubCategory
{
    public class SubCategoriesListDto
    {
        public IEnumerable<SubCategoryDto> SubCategories { get; set; } = new List<SubCategoryDto>();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public bool? IsActive { get; set; }
    }
}
