using MediatR;

namespace ERPWMS.Application.Commands.Shift.DeactivateShift
{
    public class DeactivateShiftCommand : IRequest<bool>
    {
        public int ShiftId { get; set; }
        public int CurrentUserId { get; set; }
    }
}