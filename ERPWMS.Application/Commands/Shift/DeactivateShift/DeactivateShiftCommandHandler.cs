using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using ERPWMS.Domain.Exceptions.HRMaster;
using ERPWMS.Domain.Interfaces;

namespace ERPWMS.Application.Commands.Shift.DeactivateShift
{
    public class DeactivateShiftCommandHandler : IRequestHandler<DeactivateShiftCommand, bool>
    {
        private readonly IShiftRepository _shiftRepository;

        public DeactivateShiftCommandHandler(IShiftRepository shiftRepository)
        {
            _shiftRepository = shiftRepository;
        }

        public async Task<bool> Handle(DeactivateShiftCommand request, CancellationToken cancellationToken)
        {
            var shift = await _shiftRepository.GetByIdAsync(request.ShiftId);
            if (shift == null)
                throw new ShiftNotFoundException(request.ShiftId);

            if (shift.IsDeleted == true)
                throw new ShiftDeletedException(request.ShiftId);

            if (shift.IsActive != true)
                return true;

            shift.IsActive = false;
            shift.UpdatedBy = request.CurrentUserId.ToString();
            shift.UpdatedDate = DateTime.UtcNow;

            _shiftRepository.Update(shift);
            await _shiftRepository.SaveChangesAsync();

            return true;
        }
    }
}
