using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class DashboardWidget
{
    public int WidgetId { get; set; }

    public int UserId { get; set; }

    public string? WidgetType { get; set; }

    public string WidgetConfig { get; set; } = null!;

    public int Position { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
