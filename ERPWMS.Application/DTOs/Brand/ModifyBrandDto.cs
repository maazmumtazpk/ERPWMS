namespace ERPWMS.Application.DTOs.Brand
{
    public class ModifyBrandDto
    {
        public int BrandId { get; set; }
        public string BrandName { get; set; } = null!;
        public string? BrandCode { get; set; }
        public string? LogoURL { get; set; }
        public bool? IsActive { get; set; }
    }
}
