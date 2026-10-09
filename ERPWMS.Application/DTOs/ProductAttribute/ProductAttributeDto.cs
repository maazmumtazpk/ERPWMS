using System;

namespace ERPWMS.Application.DTOs.ProductAttribute
{
    public class ProductAttributeDto
    {
        public int AttributeId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public string AttributeName { get; set; } = null!;
        public string AttributeValue { get; set; } = null!;
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
