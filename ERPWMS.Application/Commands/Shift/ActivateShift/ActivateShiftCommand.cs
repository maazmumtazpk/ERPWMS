using MediatR;

namespace ERPWMS.Application.Commands.Shift.ActivateShift
{
    public class ActivateShiftCommand : IRequest<bool>
    {
        public int ShiftId { get; set; }
        public int CurrentUserId { get; set; }
    }
}