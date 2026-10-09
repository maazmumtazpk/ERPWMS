using MediatR;

namespace ERPWMS.Application.DTOs.Warehouse
{
    public class RegisterZoneCommand : IRequest<ZoneDto>
    {
        public int WarehouseId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string ZoneCode { get; set; } = string.Empty;
    }

    public class ChangeZoneCommand : IRequest<ZoneDto>
    {
        public int ZoneId { get; set; }
        public int WarehouseId { get; set; }
        public string ZoneName { get; set; } = string.Empty;
        public string ZoneCode { get; set; } = string.Empty;
    }

    public class ActivateZoneCommand : IRequest<bool>
    {
        public int ZoneId { get; set; }
    }

    public class DeactivateZoneCommand : IRequest<bool>
    {
        public int ZoneId { get; set; }
    }

    public class DeleteZoneCommand : IRequest<bool>
    {
        public int ZoneId { get; set; }
    }
}
