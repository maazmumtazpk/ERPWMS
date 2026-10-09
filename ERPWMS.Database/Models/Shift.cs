using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Shift
{
    public int ShiftId { get; set; }

    public string ShiftName { get; set; } = null!;

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    public int? GracePeriodMinutes { get; set; }

    // =========================================================
    // STEP 5 ADDITIONS — Shift types
    // =========================================================
    public int? ShiftTypeId { get; set; }
    public bool IsNightShift { get; set; } = false;
    public bool CrossesMidnight { get; set; } = false;

    public bool? IsActive { get; set; }

    public bool? IsDeleted { get; set; }

    public string? DeletedBy { get; set; }

    public DateTime? DeletedDate { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string? DeletedByName { get; set; }

    public string? CreatedByName { get; set; }

    public string? UpdatedByName { get; set; }

    public virtual ICollection<EmployeeAttendance> EmployeeAttendances { get; set; } = new List<EmployeeAttendance>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
