using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using ERPWMS.Application.DTOs.Shift;
using ERPWMS.Domain.Interfaces;

namespace ERPWMS.Application.Queries.Shift.GetShifts
{
    public class GetShiftsQueryHandler : IRequestHandler<GetShiftsQuery, GetShiftsResult>
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IMapper _mapper;

        public GetShiftsQueryHandler(IShiftRepository shiftRepository, IMapper mapper)
        {
            _shiftRepository = shiftRepository;
            _mapper = mapper;
        }

        public async Task<GetShiftsResult> Handle(GetShiftsQuery request, CancellationToken cancellationToken)
        {
            var shifts = await _shiftRepository.GetPagedShiftsAsync(
                request.Page,
                request.PageSize,
                null,
                request.IncludeDeleted);

            var totalCount = await _shiftRepository.GetTotalCountAsync(null, request.IncludeDeleted);

            return new GetShiftsResult
            {
                Shifts = _mapper.Map<List<ShiftListDto>>(shifts),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
