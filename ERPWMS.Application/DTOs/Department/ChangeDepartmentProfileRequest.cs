using MediatR;

namespace ERPWMS.Application.DTOs.Department
{
    /// <summary>
    /// Request to change department profile
    /// </summary>
    public class ChangeDepartmentProfileRequest : IRequest<ChangeDepartmentProfileResponse>
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? DepartmentCode { get; set; }
    }

    /// <summary>
    /// Response for changing department profile
    /// </summary>
    public class ChangeDepartmentProfileResponse
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
        public string? DepartmentCode { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Message { get; set; }
    }
}