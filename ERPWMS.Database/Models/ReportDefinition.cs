using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class ReportDefinition
{
    public int ReportId { get; set; }

    public string ReportName { get; set; } = null!;

    public string ReportCode { get; set; } = null!;

    public string ReportCategory { get; set; } = null!;

    public string QueryDefinition { get; set; } = null!;

    public string? Parameters { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
