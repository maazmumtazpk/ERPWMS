using MediatR;
using ERPWMS.Application.DTOs.Product;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Product
{
    public class GetProductByIdQuery : IRequest<ProductDetailDto?>
    {
        public int ProductId { get; set; }
    }
}
