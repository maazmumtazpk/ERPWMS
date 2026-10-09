using MediatR;
using ERPWMS.Application.DTOs.Product;
using System.Collections.Generic;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Queries.Product
{
    public class GetProductsByCategoryQuery : IRequest<IEnumerable<ProductDto>>
    {
        public int CategoryId { get; set; }
        public bool OnlyActive { get; set; } = true;
    }
}
