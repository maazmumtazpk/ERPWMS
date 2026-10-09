using MediatR;
using System.Collections.Generic;

namespace ERPWMS.Application.Commands.ProductImage
{
    public class ReorderProductImagesCommand : IRequest<bool>
    {
        public int ProductId { get; set; }
        public Dictionary<int, int> ImageOrderMapping { get; set; } = new Dictionary<int, int>();
        public int UpdatedBy { get; set; }
    }
}
