using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class ErrorLog
{
    public int ErrorId { get; set; }

    public DateTime? ErrorDate { get; set; }

    public string TraceId { get; set; } = null!;

    public string ExceptionType { get; set; } = null!;

    public string Message { get; set; } = null!;

    public string? StackTrace { get; set; }

    public string? Source { get; set; }

    public int? UserId { get; set; }

    public string? IPAddress { get; set; }

    public string? RequestUrl { get; set; }

    public string? RequestMethod { get; set; }
}
