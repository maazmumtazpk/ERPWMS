using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class NotificationTemplate
{
    public int TemplateId { get; set; }

    public string TemplateName { get; set; } = null!;

    public string TemplateCode { get; set; } = null!;

    public string Subject { get; set; } = null!;

    public string Body { get; set; } = null!;

    public string? Type { get; set; }

    public bool? IsActive { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual User? CreatedByNavigation { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual User? UpdatedByNavigation { get; set; }
}
