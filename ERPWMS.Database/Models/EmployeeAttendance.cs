using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class EmployeeAttendance
{
    public int AttendanceId { get; set; }

    public int EmployeeId { get; set; }

    public DateOnly AttendanceDate { get; set; }

    public TimeOnly? CheckInTime { get; set; }

    public TimeOnly? CheckOutTime { get; set; }

    public string? Status { get; set; }

    public decimal? OvertimeHours { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public int? ShiftId { get; set; }

    public bool IsDeleted { get; set; }

    public virtual Shift? Shift { get; set; }
    // =========================================================
    public virtual Employee? Employee { get; set; }

    // =========================================================
    // STEP 6 ADDITIONS — Auto Attendance fields
    // =========================================================

    public int? LateMinutes { get; set; }
    public int? EarlyMinutes { get; set; }
    public decimal? OTHours { get; set; }
    public int? WorkingMinutes { get; set; }
    public bool? IsHalfDay { get; set; }
    public bool? IsHolidayWork { get; set; }
    public bool? IsWeeklyOffWork { get; set; }
    public string? Remarks { get; set; }

}



