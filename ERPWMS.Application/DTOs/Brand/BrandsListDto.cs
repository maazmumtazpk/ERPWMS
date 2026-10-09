using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Brand
{
    public class BrandsListDto
    {
        public IEnumerable<BrandDto> Brands { get; set; } = new List<BrandDto>();
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
    }
}
