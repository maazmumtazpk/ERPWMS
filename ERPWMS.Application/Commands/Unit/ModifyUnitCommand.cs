using MediatR;

namespace ERPWMS.Application.Commands.Unit
{
    public class ModifyUnitCommand : IRequest<bool>
    {
        public int UnitId { get; set; }

        public string UnitName { get; set; } = string.Empty;

        public string? UnitCode { get; set; }

        public bool IsBaseUnit { get; set; }

        public decimal? ConversionFactor { get; set; }

        public int? BaseUnitId { get; set; }

        public bool? IsActive { get; set; }

        public int UpdatedBy { get; set; }
    }
}