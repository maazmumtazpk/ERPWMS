using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class Notification
{
    public int NotificationId { get; set; }

    public int UserId { get; set; }

    public int? TemplateId { get; set; }

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? SentDate { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? ReadDate { get; set; }

    public string? Type { get; set; }

    public string? ReferenceNo { get; set; }

    public string? ReferenceModule { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual NotificationTemplate? Template { get; set; }

    public virtual User User { get; set; } = null!;
}
