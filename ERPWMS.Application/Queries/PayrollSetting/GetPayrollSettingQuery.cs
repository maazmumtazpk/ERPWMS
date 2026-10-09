using MediatR;
using ERPWMS.Application.DTOs.PayrollSetting;

namespace ERPWMS.Application.Queries.PayrollSetting
{
    public class GetPayrollSettingQuery : IRequest<PayrollSettingDto>
    {
    }
}
