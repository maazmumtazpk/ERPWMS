using AutoMapper;
using ERPWMS.Application.DTOs.Shift;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

using ShiftEntity = ERPWMS.Database.Models.Shift;

namespace ERPWMS.Application.Commands.Shift.Create_Shift
{
    public class CreateShiftCommandHandler
        : IRequestHandler<CreateShiftCommand, ShiftDto>
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IMapper _mapper;

        public CreateShiftCommandHandler(
            IShiftRepository shiftRepository,
            IMapper mapper)
        {
            _shiftRepository = shiftRepository;
            _mapper = mapper;
        }

        public async Task<ShiftDto> Handle(
            CreateShiftCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Request == null)
                throw new ArgumentNullException(nameof(request.Request));

            if (string.IsNullOrWhiteSpace(request.Request.ShiftName))
                throw new ArgumentException("Shift name is required.", nameof(request.Request.ShiftName));

            var shiftName = request.Request.ShiftName.Trim();

            if (!request.Request.StartTime.HasValue)
                throw new ArgumentException("Start time is required.", nameof(request.Request.StartTime));

            if (!request.Request.EndTime.HasValue)
                throw new ArgumentException("End time is required.", nameof(request.Request.EndTime));

            var startTime = request.Request.StartTime.Value;
            var endTime = request.Request.EndTime.Value;

            if (startTime >= endTime)
                throw new ShiftTimeConflictException(startTime, endTime);

            var shiftExists = await _shiftRepository.ShiftNameExistsAsync(shiftName);

            if (shiftExists)
                throw new ShiftAlreadyExistsException(shiftName);

            var shift = new ShiftEntity
            {
                ShiftName = shiftName,
                StartTime = startTime,
                EndTime = endTime,
                GracePeriodMinutes = request.Request.GracePeriodMinutes,
                IsActive = true,
                IsDeleted = false,

                // ✅ FIX: int → string
                CreatedBy = request.CurrentUserId.ToString(),
                CreatedDate = DateTime.UtcNow
            };

            await _shiftRepository.AddAsync(shift);
            await _shiftRepository.SaveChangesAsync();

            var result = _mapper.Map<ShiftDto>(shift);
            return result;
        }
    }
}