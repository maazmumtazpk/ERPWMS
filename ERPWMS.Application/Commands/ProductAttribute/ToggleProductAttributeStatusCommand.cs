using MediatR;

namespace ERPWMS.Application.Commands.ProductAttribute
{
    public class ToggleProductAttributeStatusCommand : IRequest<bool>
    {
        public int AttributeId { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }
}
