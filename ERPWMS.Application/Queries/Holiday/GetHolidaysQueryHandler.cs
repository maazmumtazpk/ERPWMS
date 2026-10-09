using AutoMapper;
using ERPWMS.Application.DTOs.Holiday;
using ERPWMS.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Queries.Holiday
{
    public class GetHolidaysQueryHandler
        : IRequestHandler<GetHolidaysQuery, List<HolidayDto>>
    {
        private readonly IHolidayRepository _repo;
        private readonly IMapper _mapper;

        public GetHolidaysQueryHandler(IHolidayRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<HolidayDto>> Handle(GetHolidaysQuery request, CancellationToken ct)
        {
            var list = await _repo.GetAllAsync(request.Year, request.BranchId, request.IncludeInactive);
            return _mapper.Map<List<HolidayDto>>(list);
        }
    }
}
