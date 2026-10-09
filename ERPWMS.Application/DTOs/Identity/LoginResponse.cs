namespace ERPWMS.Application.DTOs.Identity
{
    public class LoginResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? RedirectUrl { get; set; }
        public int UserId { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string[]? Roles { get; set; }
        public string? Token { get; set; } // if using JWT; otherwise not used
    }
}