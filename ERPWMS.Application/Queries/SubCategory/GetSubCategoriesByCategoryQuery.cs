using MediatR;
using ERPWMS.Application.DTOs.SubCategory;
using System.Collections.Generic;

namespace ERPWMS.Application.Queries.SubCategory
{
    public class GetSubCategoriesByCategoryQuery : IRequest<IEnumerable<SubCategoryDto>>
    {
        public int CategoryId { get; set; }
        public bool OnlyActive { get; set; } = true;
    }
}
