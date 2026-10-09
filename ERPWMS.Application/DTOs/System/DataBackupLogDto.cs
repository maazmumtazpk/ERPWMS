using System;

namespace ERPWMS.Application.DTOs.System
{
    public class DataBackupLogDto
    {
        public int BackupId { get; set; }
        public DateTime? BackupDate { get; set; }
        public string? BackupType { get; set; }
        public decimal FileSizeMB { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string? Status { get; set; }
        public string? ErrorMessage { get; set; }
        public string CreatedBy { get; set; }
        public string? CreatedByUsername { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string StatusDisplay => Status == "Success" ? "✅ Success" : Status == "Failed" ? "❌ Failed" : "⏳ Pending";
        public string FileSizeDisplay => FileSizeMB > 1024 ? $"{FileSizeMB / 1024:F2} GB" : $"{FileSizeMB:F2} MB";
    }
}