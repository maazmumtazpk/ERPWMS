using MediatR;
using ERPWMS.Application.DTOs.ProductAttribute;
using System.Collections.Generic;

namespace ERPWMS.Application.Queries.ProductAttribute
{
    public class GetProductAttributesQuery : IRequest<IEnumerable<ProductAttributeDto>>
    {
        public int ProductId { get; set; }
        public bool OnlyActive { get; set; } = true;
    }
}
