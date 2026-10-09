using System;

namespace ERPWMS.Application.DTOs.SubCategory
{
    public class SubCategoryDto
    {
        public int SubCategoryId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public string SubCategoryName { get; set; } = null!;
        public string? SubCategoryCode { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string?DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int ProductCount { get; set; }
    }
}
