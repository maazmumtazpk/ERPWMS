using MediatR;
using ERPWMS.Application.DTOs.Shift;

namespace ERPWMS.Application.Commands.Shift.ChangeShift
{
    public class ChangeShiftCommand : IRequest<ShiftDto>
    {
        public int ShiftId { get; set; }
        public CreateShiftRequest Request { get; set; } = null!;
        public int CurrentUserId { get; set; }
    }
}