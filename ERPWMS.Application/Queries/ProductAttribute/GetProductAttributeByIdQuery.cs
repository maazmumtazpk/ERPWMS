using MediatR;
using ERPWMS.Application.DTOs.ProductAttribute;

namespace ERPWMS.Application.Queries.ProductAttribute
{
    public class GetProductAttributeByIdQuery : IRequest<ProductAttributeDto?>
    {
        public int AttributeId { get; set; }
    }
}
