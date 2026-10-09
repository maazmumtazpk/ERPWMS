using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Brand
{
    public class ToggleBrandStatusCommand : IRequest<bool>
    {
        public int BrandId { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }
}
