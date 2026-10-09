using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class UserSession
{
    public int SessionId { get; set; }

    public int UserId { get; set; }

    public string SessionToken { get; set; } = null!;

    public DateTime? LoginTime { get; set; }

    public DateTime? LastActivity { get; set; }

    public string? IPAddress { get; set; }

    public string? DeviceInfo { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? LogoutTime { get; set; }

    public virtual User User { get; set; } = null!;
}
