using MediatR;

namespace ERPWMS.Application.Commands.Unit
{
    public class DeleteUnitCommand : IRequest<bool>
    {
        public int UnitId { get; set; }
        public int DeletedBy { get; set; }
    }
}
