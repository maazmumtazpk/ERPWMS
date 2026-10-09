using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Product
{
    public class ToggleProductStatusCommand : IRequest<bool>
    {
        public int ProductId { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }
}
