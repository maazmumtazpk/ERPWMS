using MediatR;
using ERPWMS.Application.DTOs.ProductImage;
using System.Collections.Generic;

namespace ERPWMS.Application.Queries.ProductImage
{
    public class GetProductImagesQuery : IRequest<IEnumerable<ProductImageDto>>
    {
        public int ProductId { get; set; }
        public bool OnlyActive { get; set; } = true;
    }
}
