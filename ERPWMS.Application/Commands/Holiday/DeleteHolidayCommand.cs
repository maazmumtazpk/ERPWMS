using MediatR;

namespace ERPWMS.Application.Commands.Holiday
{
    public class DeleteHolidayCommand : IRequest<bool>
    {
        public int HolidayId { get; set; }
        public int CurrentUserId { get; set; }
    }
}
