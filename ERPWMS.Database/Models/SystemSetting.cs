using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class SystemSetting
{
    public int SettingId { get; set; }

    public string SettingKey { get; set; } = null!;

    public string SettingValue { get; set; } = null!;

    public string? DataType { get; set; }

    public bool? IsEncrypted { get; set; }

    public string? Description { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
