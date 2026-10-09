using MediatR;

namespace ERPWMS.Application.DTOs.Designation
{
    public class ChangeDesignationProfileRequest : IRequest<ChangeDesignationProfileResponse>
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public int? RankLevel { get; set; }
    }

    public class ChangeDesignationProfileResponse
    {
        public int DesignationId { get; set; }
        public string DesignationName { get; set; } = string.Empty;
        public int? RankLevel { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? Message { get; set; }
    }
}

