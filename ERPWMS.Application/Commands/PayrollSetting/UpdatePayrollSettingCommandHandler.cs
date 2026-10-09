using AutoMapper;
using ERPWMS.Application.DTOs.PayrollSetting;
using ERPWMS.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

using PayrollSettingEntity = ERPWMS.Database.Models.PayrollSetting;

namespace ERPWMS.Application.Commands.PayrollSetting
{
    public class UpdatePayrollSettingCommandHandler
        : IRequestHandler<UpdatePayrollSettingCommand, PayrollSettingDto>
    {
        private readonly IPayrollSettingRepository _repo;
        private readonly IMapper _mapper;

        public UpdatePayrollSettingCommandHandler(
            IPayrollSettingRepository repo,
            IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<PayrollSettingDto> Handle(
            UpdatePayrollSettingCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null || request.Request == null)
                throw new ArgumentNullException(nameof(request));

            var dto = request.Request;

            var entity = await _repo.GetSettingAsync();
            if (entity == null)
            {
                entity = new PayrollSettingEntity
                {
                    CreatedDate = DateTime.UtcNow
                };
                _mapper.Map(dto, entity);
                entity.CreatedBy = request.CurrentUserId.ToString();
                await _repo.AddAsync(entity);
            }
            else
            {
                _mapper.Map(dto, entity);
                entity.UpdatedBy = request.CurrentUserId.ToString();
                entity.UpdatedDate = DateTime.UtcNow;
                _repo.Update(entity);
            }

            await _repo.SaveChangesAsync();
            return _mapper.Map<PayrollSettingDto>(entity);
        }
    }
}
