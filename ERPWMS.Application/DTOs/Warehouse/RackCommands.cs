using MediatR;

namespace ERPWMS.Application.DTOs.Warehouse
{
    public class RegisterRackCommand : IRequest<RackDto>
    {
        public int WarehouseId { get; set; }
        public int? ZoneId { get; set; }
        public string RackName { get; set; } = string.Empty;
        public string RackCode { get; set; } = string.Empty;
    }

    public class ChangeRackCommand : IRequest<RackDto>
    {
        public int RackId { get; set; }
        public int ZoneId { get; set; }
        public string RackName { get; set; } = string.Empty;
        public string RackCode { get; set; } = string.Empty;
    }

    public class ActivateRackCommand : IRequest<bool>
    {
        public int RackId { get; set; }
    }

    public class DeactivateRackCommand : IRequest<bool>
    {
        public int RackId { get; set; }
    }

    public class DeleteRackCommand : IRequest<bool>
    {
        public int RackId { get; set; }
    }
}