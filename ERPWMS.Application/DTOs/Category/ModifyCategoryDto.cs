namespace ERPWMS.Application.DTOs.Category
{
    public class ModifyCategoryDto
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string? CategoryCode { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}
