namespace ERPWMS.Domain.Services.DTOs
{
    public class GetLeaveBalanceRequest
    {
        public int EmployeeId { get; set; }
        public int? LeaveTypeId { get; set; }
        public int? Year { get; set; }
    }
}