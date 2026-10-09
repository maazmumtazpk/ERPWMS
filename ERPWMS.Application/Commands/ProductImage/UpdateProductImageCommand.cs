using MediatR;

namespace ERPWMS.Application.Commands.ProductImage
{
    public class UpdateProductImageCommand : IRequest<bool>
    {
        public int ImageId { get; set; }
        public string ImageURL { get; set; } = null!;
        public bool? IsPrimary { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
        public String?UpdatedBy{ get; set; }
    }
}
