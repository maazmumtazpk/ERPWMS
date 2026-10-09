using MediatR;
using ERPWMS.Application.DTOs.PayrollSetting;

namespace ERPWMS.Application.Commands.PayrollSetting
{
    public class UpdatePayrollSettingCommand : IRequest<PayrollSettingDto>
    {
        public PayrollSettingDto Request { get; set; } = null!;
        public int CurrentUserId { get; set; }
    }
}
