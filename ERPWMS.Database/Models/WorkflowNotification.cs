using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class WorkflowNotification
{
    public int NotificationId { get; set; }

    public int WorkflowInstId { get; set; }

    public int UserId { get; set; }

    public string Subject { get; set; } = null!;

    public string Message { get; set; } = null!;

    public DateTime? SentDate { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? ReadDate { get; set; }

    public string? NotificationType { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual WorkflowInstance WorkflowInst { get; set; } = null!;
}
