using System;

namespace ERPWMS.Application.DTOs.Holiday
{
    public class HolidayDto
    {
        public int HolidayId { get; set; }
        public string HolidayName { get; set; } = null!;
        public DateOnly HolidayDate { get; set; }
        public string HolidayType { get; set; } = "Public";
        public string Scope { get; set; } = "All";
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public bool IsPaid { get; set; }
        public bool IsRecurring { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}
