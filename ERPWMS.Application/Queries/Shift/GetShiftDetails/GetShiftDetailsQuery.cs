using MediatR;
using ERPWMS.Application.DTOs.Shift;

namespace ERPWMS.Application.Queries.Shift.GetShiftDetails
{
    public class GetShiftDetailsQuery : IRequest<ShiftDto>
    {
        public int ShiftId { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}