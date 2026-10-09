using MediatR;

namespace ERPWMS.Application.DTOs.Warehouse
{
    public class RegisterWarehouseCommand : IRequest<WarehouseDetailsDto>
    {
        public string WarehouseName { get; set; } = string.Empty;
        public string WarehouseCode { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ChangeWarehouseProfileCommand : IRequest<WarehouseDetailsDto>
    {
        public int WarehouseId { get; set; }
        public string WarehouseName { get; set; } = string.Empty;
        public string WarehouseCode { get; set; } = string.Empty;
        public int BranchId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class ActivateWarehouseCommand : IRequest<bool>
    {
        public int WarehouseId { get; set; }
    }

    public class DeactivateWarehouseCommand : IRequest<bool>
    {
        public int WarehouseId { get; set; }
    }

    public class DeleteWarehouseCommand : IRequest<bool>
    {
        public int WarehouseId { get; set; }
    }
}
