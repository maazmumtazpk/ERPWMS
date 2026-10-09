using MediatR;

namespace ERPWMS.Application.Commands.Finance.TaxSetting
{
    public class DeleteTaxSettingCommand : IRequest<DeleteTaxSettingResponse>
    {
        public int TaxId { get; set; }
        public int CompanyId { get; set; }
        public int DeletedBy { get; set; }
    }

    public class DeleteTaxSettingResponse
    {
        public bool IsSuccess { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }
}
