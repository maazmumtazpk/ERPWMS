using MediatR;

namespace ERPWMS.Application.DTOs.Warehouse
{
    public class RegisterBinCommand : IRequest<BinDto>
    {
        // ✅ Added these two
        public int WarehouseId { get; set; }
        public int? ZoneId { get; set; }

        public int RackId { get; set; }
        public string BinName { get; set; } = string.Empty;
        public string BinCode { get; set; } = string.Empty;
        public bool? IsOccupied { get; set; }  // optional, kept as is
    }

    public class ChangeBinCommand : IRequest<BinDto>
    {
        public int BinId { get; set; }
        public int RackId { get; set; }
        public string BinName { get; set; } = string.Empty;
        public string BinCode { get; set; } = string.Empty;
        public bool? IsOccupied { get; set; }
    }

    public class ActivateBinCommand : IRequest<bool>
    {
        public int BinId { get; set; }
    }

    public class DeactivateBinCommand : IRequest<bool>
    {
        public int BinId { get; set; }
    }

    public class DeleteBinCommand : IRequest<bool>
    {
        public int BinId { get; set; }
    }

    public class MarkBinOccupiedCommand : IRequest<bool>
    {
        public int BinId { get; set; }
    }

    public class MarkBinAvailableCommand : IRequest<bool>
    {
        public int BinId { get; set; }
    }
}