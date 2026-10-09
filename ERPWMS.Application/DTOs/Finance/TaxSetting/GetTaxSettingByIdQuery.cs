using MediatR;

namespace ERPWMS.Application.DTOs.Finance.TaxSetting
{
    public class GetTaxSettingByIdQuery : IRequest<TaxSettingResponse?>
    {
        public int TaxId { get; set; }
        public int CompanyId { get; set; }
    }
}
