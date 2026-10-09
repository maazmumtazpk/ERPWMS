using MediatR;
using ERPWMS.Application.DTOs.Unit;

namespace ERPWMS.Application.Queries.Unit
{
    public class GetUnitsQuery : IRequest<UnitsListDto>
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; }

        public bool? IsActive { get; set; }

        public bool? IsBaseUnit { get; set; }

        public bool IncludeDeleted { get; set; } = false;
    }
}