using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class LoginHistory
{
    public int LoginId { get; set; }

    public int UserId { get; set; }

    public DateTime? LoginTime { get; set; }

    public DateTime? LogoutTime { get; set; }

    public string? IPAddress { get; set; }

    public string? DeviceInfo { get; set; }

    public string? Status { get; set; }

    public virtual User User { get; set; } = null!;
}
