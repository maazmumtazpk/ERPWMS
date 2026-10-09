using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class ScheduledReport
{
    public int ScheduleId { get; set; }

    public int ReportId { get; set; }

    public string? Frequency { get; set; }

    public TimeOnly ScheduleTime { get; set; }

    public string? ScheduleDays { get; set; }

    public string Recipients { get; set; } = null!;

    public DateTime? LastRunDate { get; set; }

    public DateTime? NextRunDate { get; set; }

    public string? Status { get; set; }

    public string? Parameters { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
