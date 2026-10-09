using MediatR;
using ERPWMS.Application.DTOs.Product;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Product
{
    public class GetProductBySKUQuery : IRequest<ProductDto?>
    {
        public string SKU { get; set; } = null!;
    }
}
