using MediatR;

namespace ERPWMS.Application.Commands.ProductImage
{
    public class SetPrimaryImageCommand : IRequest<bool>
    {
        public int ImageId { get; set; }
        public int ProductId { get; set; }
        public int UpdatedBy { get; set; }
    }
}
