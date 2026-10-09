namespace ERPWMS.Domain.Services.DTOs
{
    public class LeaveBalanceDetails
    {
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public string? LeaveTypeName { get; set; }
        public int TotalEntitled { get; set; }
        public int Used { get; set; }
        public int Pending { get; set; }
        public int Remaining { get; set; }
        public int Year { get; set; }
    }
}