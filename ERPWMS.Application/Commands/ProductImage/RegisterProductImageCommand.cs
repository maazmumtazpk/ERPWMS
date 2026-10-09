using MediatR;

namespace ERPWMS.Application.Commands.ProductImage
{
    public class RegisterProductImageCommand : IRequest<int>
    {
        public int ProductId { get; set; }
        public string ImageURL { get; set; } = null!;
        public bool? IsPrimary { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
    }
}
