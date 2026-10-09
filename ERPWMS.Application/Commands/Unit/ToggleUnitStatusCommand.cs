using MediatR;

namespace ERPWMS.Application.Commands.Unit
{
    public class ToggleUnitStatusCommand : IRequest<bool>
    {
        public int UnitId { get; set; }
        public bool IsActive { get; set; }
        public int UpdatedBy { get; set; }
    }
}
