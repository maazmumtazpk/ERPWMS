using System;

namespace ERPWMS.Application.DTOs.Identity
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? EmployeeCode { get; set; }
        public int? WarehouseId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string[]? Roles { get; set; }
    }
}