using Microsoft.AspNetCore.Http;

namespace ERPWMS.Application.DTOs.ProductImage
{
    public class UploadProductImageDto
    {
        public int ProductId { get; set; }
        public IFormFile ImageFile { get; set; } = null!;
        public bool? IsPrimary { get; set; }
        public int? DisplayOrder { get; set; }
    }
}
