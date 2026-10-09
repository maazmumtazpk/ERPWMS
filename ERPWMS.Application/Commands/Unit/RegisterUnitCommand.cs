using MediatR;

namespace ERPWMS.Application.Commands.Unit
{
    public class RegisterUnitCommand : IRequest<int>
    {
        public string UnitName { get; set; } = null!;
        public string? UnitCode { get; set; }
        public bool? IsBaseUnit { get; set; }
        public decimal? ConversionFactor { get; set; }
        public int? BaseUnitId { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
    }
}
