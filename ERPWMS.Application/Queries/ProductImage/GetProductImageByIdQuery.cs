using MediatR;
using ERPWMS.Application.DTOs.ProductImage;

namespace ERPWMS.Application.Queries.ProductImage
{
    public class GetProductImageByIdQuery : IRequest<ProductImageDto?>
    {
        public int ImageId { get; set; }
    }
}

