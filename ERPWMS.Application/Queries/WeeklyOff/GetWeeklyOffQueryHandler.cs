using AutoMapper;
using ERPWMS.Application.DTOs.WeeklyOff;
using ERPWMS.Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Queries.WeeklyOff
{
    public class GetWeeklyOffQueryHandler
        : IRequestHandler<GetWeeklyOffQuery, List<WeeklyOffDto>>
    {
        private readonly IWeeklyOffRepository _repo;
        private readonly IMapper _mapper;

        public GetWeeklyOffQueryHandler(IWeeklyOffRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<List<WeeklyOffDto>> Handle(GetWeeklyOffQuery request, CancellationToken ct)
        {
            var list = await _repo.GetAllAsync(request.EmployeeId);
            return _mapper.Map<List<WeeklyOffDto>>(list);
        }
    }
}
