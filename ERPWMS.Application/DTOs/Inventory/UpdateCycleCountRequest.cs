using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Inventory
{
    public class UpdateCycleCountRequest
    {
        public int CycleCountId { get; set; }

        public int WarehouseId { get; set; }

        public int ProductId { get; set; }

        public int BinId { get; set; }

        public string? Status { get; set; }

        public List<UpdateCycleCountDetailRequest> CycleCountDetails { get; set; } = [];
    }
}