using MediatR;

namespace ERPWMS.Application.DTOs.Branch
{
    /// <summary>
    /// Request to change branch profile
    /// </summary>
    public class ChangeBranchProfileRequest : IRequest<ChangeBranchProfileResponse>
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string? BranchCode { get; set; }
        public string? Address { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
    }

    /// <summary>
    /// Response for changing branch profile
    /// </summary>
    public class ChangeBranchProfileResponse
    {
        public int BranchId { get; set; }
        public string BranchName { get; set; } = string.Empty;
        public string? BranchCode { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Message { get; set; }
    }
}