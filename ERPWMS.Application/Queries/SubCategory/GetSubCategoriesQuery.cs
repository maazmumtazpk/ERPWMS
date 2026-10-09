using MediatR;
using ERPWMS.Application.DTOs.SubCategory;

namespace ERPWMS.Application.Queries.SubCategory
{
    public class GetSubCategoriesQuery : IRequest<SubCategoriesListDto>
    {
        public string? SearchTerm { get; set; }
        public int? CategoryId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public bool? IsActive { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}
