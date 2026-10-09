namespace ERPWMS.Application.DTOs.Identity
{
    public class ModuleDto
    {
        public int ModuleId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string ModuleCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
    }
}