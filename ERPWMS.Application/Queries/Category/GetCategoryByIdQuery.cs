using MediatR;
using ERPWMS.Application.DTOs.Category;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Category
{
    public class GetCategoryByIdQuery : IRequest<CategoryDto?>
    {
        public int CategoryId { get; set; }
    }
}
