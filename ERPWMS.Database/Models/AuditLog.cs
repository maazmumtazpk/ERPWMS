using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class AuditLog
{
    public int AuditId { get; set; }

    public int UserId { get; set; }

    public int? ModuleId { get; set; }

    public string Action { get; set; } = null!;

    public int RecordId { get; set; }

    public string RecordType { get; set; } = null!;

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public string? IPAddress { get; set; }

    public string? UserAgent { get; set; }

    public DateTime? Timestamp { get; set; }

    public virtual Module? Module { get; set; }

    public virtual User User { get; set; } = null!;
}
