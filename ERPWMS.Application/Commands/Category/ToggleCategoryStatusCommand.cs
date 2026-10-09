using MediatR;
using ERPWMS.Database.Models;

namespace ERPWMS.Application.Commands.Category
{
    public class ToggleCategoryStatusCommand : IRequest<bool>
    {
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }
}
