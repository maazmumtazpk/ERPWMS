using AutoMapper;
using ERPWMS.Application.DTOs.Holiday;
using ERPWMS.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Queries.Holiday
{
    public class GetHolidayByDateQueryHandler
        : IRequestHandler<GetHolidayByDateQuery, HolidayDto?>
    {
        private readonly IHolidayRepository _repo;
        private readonly IMapper _mapper;

        public GetHolidayByDateQueryHandler(IHolidayRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<HolidayDto?> Handle(GetHolidayByDateQuery request, CancellationToken ct)
        {
            var e = await _repo.GetByDateAsync(request.Date, request.BranchId);
            return e == null ? null : _mapper.Map<HolidayDto>(e);
        }
    }
}
