namespace ERPWMS.Application.DTOs.Unit
{
    public class ModifyUnitDto
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; } = null!;
        public string? UnitCode { get; set; }
        public bool? IsBaseUnit { get; set; }
        public decimal? ConversionFactor { get; set; }
        public int? BaseUnitId { get; set; }
        public bool? IsActive { get; set; }
    }
}
