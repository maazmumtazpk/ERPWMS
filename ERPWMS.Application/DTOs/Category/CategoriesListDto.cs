using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Category
{
    public class CategoriesListDto
    {
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }
}
