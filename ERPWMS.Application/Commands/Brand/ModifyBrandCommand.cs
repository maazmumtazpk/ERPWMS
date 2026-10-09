using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Brand
{
    public class ModifyBrandCommand : IRequest<bool>
    {
        public int BrandId { get; set; }

        public string BrandName { get; set; } = string.Empty;

        public string? BrandCode { get; set; }

        public string? LogoURL { get; set; }

        public bool? IsActive { get; set; }

        public int UpdatedBy { get; set; }
    }
}