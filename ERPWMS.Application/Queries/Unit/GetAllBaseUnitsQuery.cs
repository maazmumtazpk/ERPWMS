using MediatR;
using ERPWMS.Application.DTOs.Unit;
using System.Collections.Generic;

namespace ERPWMS.Application.Queries.Unit
{
    public class GetAllBaseUnitsQuery : IRequest<IEnumerable<UnitDto>>
    {
        public bool OnlyActive { get; set; } = true;
    }
}
