
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using AutoMapper;
using ERPWMS.Application.DTOs.Shift;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;

namespace ERPWMS.Application.Queries.Shift.GetShiftDetails
{
    public class GetShiftDetailsQueryHandler : IRequestHandler<GetShiftDetailsQuery, ShiftDto>
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IMapper _mapper;

        public GetShiftDetailsQueryHandler(IShiftRepository shiftRepository, IMapper mapper)
        {
            _shiftRepository = shiftRepository;
            _mapper = mapper;
        }

        public async Task<ShiftDto> Handle(GetShiftDetailsQuery request, CancellationToken cancellationToken)
        {
            var shift = await _shiftRepository.GetByIdWithDetailsAsync(request.ShiftId);
            if (shift == null)
                throw new ShiftNotFoundException(request.ShiftId);

            if (!request.IncludeDeleted && shift.IsDeleted == true)
                throw new ShiftNotFoundException(request.ShiftId);

            return _mapper.Map<ShiftDto>(shift);
        }
    }
}
