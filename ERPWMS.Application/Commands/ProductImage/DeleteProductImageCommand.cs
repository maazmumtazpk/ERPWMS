using MediatR;

namespace ERPWMS.Application.Commands.ProductImage
{
    public class DeleteProductImageCommand : IRequest<bool>
    {
        public int ImageId { get; set; }
        public int DeletedBy { get; set; }
    }
}
