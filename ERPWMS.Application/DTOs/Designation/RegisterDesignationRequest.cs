using MediatR;

namespace ERPWMS.Application.DTOs.Designation
{
    public class RegisterDesignationRequest : IRequest<RegisterDesignationResponse>
    {
        public string DesignationName { get; set; } = string.Empty;
        public int? RankLevel { get; set; }
        public int DepartmentId { get; set; }
    }

    public class RegisterDesignationResponse
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public int? RankLevel { get; set; }
        public int DepartmentId { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? Message { get; set; }
    }
}