using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Brand
{
    public class RegisterBrandCommand : IRequest<int>
    {
        public string BrandName { get; set; } = null!;
        public string? BrandCode { get; set; }
        public string? LogoURL { get; set; }
        public bool? IsActive { get; set; }
        public string CreatedBy { get; set; }
    }
}
