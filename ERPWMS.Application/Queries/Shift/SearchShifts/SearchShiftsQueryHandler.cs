using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using ERPWMS.Application.DTOs.Shift;
using ERPWMS.Domain.Interfaces;

namespace ERPWMS.Application.Queries.Shift.SearchShifts
{
    public class SearchShiftsQueryHandler : IRequestHandler<SearchShiftsQuery, SearchShiftsResult>
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IMapper _mapper;

        public SearchShiftsQueryHandler(IShiftRepository shiftRepository, IMapper mapper)
        {
            _shiftRepository = shiftRepository;
            _mapper = mapper;
        }

        public async Task<SearchShiftsResult> Handle(SearchShiftsQuery request, CancellationToken cancellationToken)
        {
            var shifts = await _shiftRepository.GetPagedShiftsAsync(
                request.Page,
                request.PageSize,
                request.SearchTerm,
                request.IncludeDeleted);

            if (request.IsActive.HasValue)
                shifts = shifts.Where(s => s.IsActive == request.IsActive.Value);

            var totalCount = await _shiftRepository.GetTotalCountAsync(request.SearchTerm, request.IncludeDeleted);

            return new SearchShiftsResult
            {
                Shifts = _mapper.Map<List<ShiftListDto>>(shifts),
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }
}
