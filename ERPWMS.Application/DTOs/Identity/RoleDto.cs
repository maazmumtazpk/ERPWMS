namespace ERPWMS.Application.DTOs.Identity
{
    public class RoleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NormalizedName { get; set; } = string.Empty;
        public string? ConcurrencyStamp { get; set; }
        public bool IsSystemRole { get; set; } // optional, derived
        public int UserCount { get; set; } // for UI
    }
}