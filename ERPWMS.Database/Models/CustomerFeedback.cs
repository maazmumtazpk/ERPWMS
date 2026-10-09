using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class CustomerFeedback
{
    public int FeedbackId { get; set; }

    public int CustomerId { get; set; }

    public DateTime? FeedbackDate { get; set; }

    public int? Rating { get; set; }

    public string? Comments { get; set; }

    public string? Category { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
