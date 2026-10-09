using MediatR;
using ERPWMS.Application.DTOs.Shift;

namespace ERPWMS.Application.Commands.Shift.Create_Shift
{
    public class CreateShiftCommand : IRequest<ShiftDto>
    {
        public CreateShiftRequest Request { get; set; } = null!;

        public int CurrentUserId { get; set; }
    }
}