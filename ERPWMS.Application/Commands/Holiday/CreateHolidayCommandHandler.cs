using AutoMapper;
using ERPWMS.Application.DTOs.Holiday;
using ERPWMS.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

using HolidayEntity = ERPWMS.Database.Models.Holiday;

namespace ERPWMS.Application.Commands.Holiday
{
    public class CreateHolidayCommandHandler
        : IRequestHandler<CreateHolidayCommand, HolidayDto>
    {
        private readonly IHolidayRepository _repo;
        private readonly IMapper _mapper;

        public CreateHolidayCommandHandler(IHolidayRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<HolidayDto> Handle(CreateHolidayCommand request, CancellationToken ct)
        {
            if (request?.Request == null) throw new ArgumentNullException(nameof(request));

            var dto = request.Request;

            var entity = _mapper.Map<HolidayEntity>(dto);
            entity.IsActive = true;
            entity.IsDeleted = false;
            entity.CreatedBy = request.CurrentUserId.ToString();
            entity.CreatedDate = DateTime.UtcNow;

            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();

            return _mapper.Map<HolidayDto>(entity);
        }
    }
}
