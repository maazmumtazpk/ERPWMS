using MediatR;
using ERPWMS.Application.DTOs.SubCategory;

namespace ERPWMS.Application.Queries.SubCategory
{
    public class GetSubCategoryByIdQuery : IRequest<SubCategoryDto?>
    {
        public int SubCategoryId { get; set; }
    }
}
