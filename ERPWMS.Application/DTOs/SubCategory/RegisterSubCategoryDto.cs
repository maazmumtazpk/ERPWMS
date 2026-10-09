
using System.ComponentModel.DataAnnotations;

namespace ERPWMS.Application.DTOs.SubCategory
{
    public class RegisterSubCategoryDto
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string SubCategoryName { get; set; } = null!;

        public string? SubCategoryCode { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; } = true;
    }
}