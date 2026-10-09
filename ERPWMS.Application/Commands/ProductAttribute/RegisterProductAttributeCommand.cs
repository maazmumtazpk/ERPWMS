using MediatR;

namespace ERPWMS.Application.Commands.ProductAttribute
{
    public class RegisterProductAttributeCommand : IRequest<int>
    {
        public int ProductId { get; set; }
        public string AttributeName { get; set; } = null!;
        public string AttributeValue { get; set; } = null!;
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
    }
}
