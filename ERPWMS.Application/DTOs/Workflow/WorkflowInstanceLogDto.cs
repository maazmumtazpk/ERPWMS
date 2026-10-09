using System;

namespace ERPWMS.Application.DTOs.Workflow
{
    public class WorkflowInstanceLogDto
    {
        public int Id { get; set; }
        public int WorkflowInstanceId { get; set; }
        public int? FromStageId { get; set; }
        public string? FromStageName { get; set; }
        public int? ToStageId { get; set; }
        public string? ToStageName { get; set; }
        public int? ActionId { get; set; }
        public string? ActionName { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? Comment { get; set; }
        public string EventType { get; set; } = string.Empty; // Started, StageChanged, Completed, Rejected, etc.
        public DateTime Timestamp { get; set; }
    }
}