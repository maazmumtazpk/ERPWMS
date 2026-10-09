using MediatR;
using ERPWMS.Application.DTOs.Unit;

namespace ERPWMS.Application.Queries.Unit
{
    public class GetUnitByIdQuery : IRequest<UnitDto?>
    {
        public int UnitId { get; set; }
    }
}