namespace ERPWMS.Application.DTOs.Identity
{
    public class PermissionUpdateRequest
    {
        public int RoleId { get; set; }
        public int ModuleId { get; set; }
        public bool CanView { get; set; }
        public bool CanCreate { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanApprove { get; set; }
    }
}