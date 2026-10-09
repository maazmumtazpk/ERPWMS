using AutoMapper;
using ERPWMS.Application.DTOs.PayrollSetting;
using ERPWMS.Domain.Interfaces;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Queries.PayrollSetting
{
    public class GetPayrollSettingQueryHandler
        : IRequestHandler<GetPayrollSettingQuery, PayrollSettingDto>
    {
        private readonly IPayrollSettingRepository _repo;
        private readonly IMapper _mapper;

        public GetPayrollSettingQueryHandler(
            IPayrollSettingRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PayrollSettingDto> Handle(
            GetPayrollSettingQuery request,
            CancellationToken cancellationToken)
        {
            var entity = await _repo.GetSettingAsync();

            if (entity == null)
            {
                // Return defaults agar row nahi hai
                return new PayrollSettingDto();
            }

            return _mapper.Map<PayrollSettingDto>(entity);
        }
    }
}
