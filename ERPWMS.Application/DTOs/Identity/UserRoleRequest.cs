namespace ERPWMS.Application.DTOs.Identity
{
    public class UserRoleRequest
    {
        public int UserId { get; set; }
        public int[] RoleIds { get; set; } = Array.Empty<int>();
    }
}