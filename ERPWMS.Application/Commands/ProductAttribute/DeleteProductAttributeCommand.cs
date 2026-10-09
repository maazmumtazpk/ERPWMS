using MediatR;

namespace ERPWMS.Application.Commands.ProductAttribute
{
    public class DeleteProductAttributeCommand : IRequest<bool>
    {
        public int AttributeId { get; set; }
        public int DeletedBy { get; set; }
    }
}
