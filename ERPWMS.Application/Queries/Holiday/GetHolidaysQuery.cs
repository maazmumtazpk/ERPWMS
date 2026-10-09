using MediatR;
using System.Collections.Generic;
using ERPWMS.Application.DTOs.Holiday;

namespace ERPWMS.Application.Queries.Holiday
{
    public class GetHolidaysQuery : IRequest<List<HolidayDto>>
    {
        public int? Year { get; set; }
        public int? BranchId { get; set; }
        public bool IncludeInactive { get; set; } = false;
    }
}
