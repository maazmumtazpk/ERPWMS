using System;
using System.Collections.Generic;

namespace ERPWMS.Database.Models;

public partial class DataBackupLog
{
    public int BackupId { get; set; }

    public DateTime? BackupDate { get; set; }

    public string? BackupType { get; set; }

    public decimal FileSizeMb { get; set; }

    public string FilePath { get; set; } = null!;

    public string? Status { get; set; }

    public string? ErrorMessage { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? CreatedDate { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public virtual User? CreatedByNavigation { get; set; }
}
