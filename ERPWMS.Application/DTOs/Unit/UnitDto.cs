using System;

namespace ERPWMS.Application.DTOs.Unit
{
    public class UnitDto
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; } = null!;
        public string? UnitCode { get; set; }
        public bool? IsBaseUnit { get; set; }
        public decimal? ConversionFactor { get; set; }
        public int? BaseUnitId { get; set; }
        public string? BaseUnitName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public string?DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public String?UpdatedBy{ get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int ProductCount { get; set; }
        public int ChildUnitCount { get; set; }
    }
}
