using System;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using MediatR;

using ERPWMS.Application.DTOs.Shift;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;

namespace ERPWMS.Application.Commands.Shift.ChangeShift
{
    public class ChangeShiftCommandHandler
        : IRequestHandler<ChangeShiftCommand, ShiftDto>
    {
        private readonly IShiftRepository _shiftRepository;
        private readonly IMapper _mapper;

        public ChangeShiftCommandHandler(
            IShiftRepository shiftRepository,
            IMapper mapper)
        {
            _shiftRepository = shiftRepository;
            _mapper = mapper;
        }

        public async Task<ShiftDto> Handle(
            ChangeShiftCommand request,
            CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            if (request.Request == null)
                throw new ArgumentNullException(nameof(request.Request));

            if (request.ShiftId <= 0)
                throw new ArgumentException("Invalid Shift ID.", nameof(request.ShiftId));

            if (string.IsNullOrWhiteSpace(request.Request.ShiftName))
                throw new ArgumentException("Shift name is required.", nameof(request.Request.ShiftName));

            if (!request.Request.StartTime.HasValue)
                throw new ArgumentException("Start time is required.", nameof(request.Request.StartTime));

            if (!request.Request.EndTime.HasValue)
                throw new ArgumentException("End time is required.", nameof(request.Request.EndTime));

            TimeOnly startTime = request.Request.StartTime.Value;
            TimeOnly endTime = request.Request.EndTime.Value;

            if (startTime >= endTime)
                throw new ShiftTimeConflictException(startTime, endTime);

            if (request.Request.GracePeriodMinutes.HasValue &&
                request.Request.GracePeriodMinutes.Value < 0)
            {
                throw new ArgumentException(
                    "Grace period cannot be negative.",
                    nameof(request.Request.GracePeriodMinutes));
            }

            var shift = await _shiftRepository.GetByIdAsync(request.ShiftId);

            if (shift == null)
                throw new ShiftNotFoundException(request.ShiftId);

            if (shift.IsDeleted == true)
                throw new ShiftDeletedException(request.ShiftId);

            if (shift.IsActive != true)
                throw new ShiftInactiveException(request.ShiftId);

            var shiftName = request.Request.ShiftName.Trim();

            if (!string.Equals(shift.ShiftName, shiftName, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _shiftRepository.ShiftNameExistsAsync(
                    shiftName, request.ShiftId);

                if (exists)
                    throw new ShiftAlreadyExistsException(shiftName);
            }

            shift.ShiftName = shiftName;
            shift.StartTime = startTime;
            shift.EndTime = endTime;
            shift.GracePeriodMinutes = request.Request.GracePeriodMinutes;

            // ✅ FIX: int → string
            shift.UpdatedBy = request.CurrentUserId.ToString();
            shift.UpdatedDate = DateTime.UtcNow;

            _shiftRepository.Update(shift);
            await _shiftRepository.SaveChangesAsync();

            return _mapper.Map<ShiftDto>(shift);
        }
    }
}