using MediatR;
using System.Collections.Generic;

namespace ERPWMS.Application.DTOs.Finance.TaxSetting
{
    public class GetTaxSettingsQuery : IRequest<List<TaxSettingResponse>>
    {
        public int CompanyId { get; set; }
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDefault { get; set; }
        public bool IncludeDeleted { get; set; } = false;
    }
}
