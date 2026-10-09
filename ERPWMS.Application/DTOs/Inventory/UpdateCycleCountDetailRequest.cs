namespace ERPWMS.Application.DTOs.Inventory
{
    public class UpdateCycleCountDetailRequest
    {
        public int CountDetailId { get; set; }

        public int ProductId { get; set; }

        public int BinId { get; set; }

        public int SystemQty { get; set; }

        public int ActualQty { get; set; }

        public int Variance { get; set; }

        public string? Remarks { get; set; }
    }
}