using MediatR;

namespace ERPWMS.Application.Commands.ProductAttribute
{
    public class UpdateProductAttributeCommand : IRequest<bool>
    {
        public int AttributeId { get; set; }
        public string AttributeName { get; set; } = null!;
        public string AttributeValue { get; set; } = null!;
        public bool? IsActive { get; set; }
        public String?UpdatedBy{ get; set; }
    }
}
