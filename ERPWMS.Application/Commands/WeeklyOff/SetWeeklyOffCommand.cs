using MediatR;

namespace ERPWMS.Application.Commands.WeeklyOff
{
    public class SetWeeklyOffCommand : IRequest<int>
    {
        public int EmployeeId { get; set; }
        public string DayOfWeek { get; set; } = "Sunday";
        public string? Remarks { get; set; }
        public int CurrentUserId { get; set; }
    }
}
