using MediatR;
using ERPWMS.Application.DTOs.Holiday;

namespace ERPWMS.Application.Commands.Holiday
{
    public class CreateHolidayCommand : IRequest<HolidayDto>
    {
        public HolidayDto Request { get; set; } = null!;
        public int CurrentUserId { get; set; }
    }
}
