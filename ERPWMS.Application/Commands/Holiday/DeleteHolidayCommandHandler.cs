using ERPWMS.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERPWMS.Application.Commands.Holiday
{
    public class DeleteHolidayCommandHandler
        : IRequestHandler<DeleteHolidayCommand, bool>
    {
        private readonly IHolidayRepository _repo;

        public DeleteHolidayCommandHandler(IHolidayRepository repo) { _repo = repo; }

        public async Task<bool> Handle(DeleteHolidayCommand request, CancellationToken ct)
        {
            var entity = await _repo.GetByIdAsync(request.HolidayId);
            if (entity == null) return false;

            entity.IsDeleted = true;
            entity.IsActive = false;
            entity.DeletedBy = request.CurrentUserId.ToString();
            entity.DeletedDate = DateTime.UtcNow;

            _repo.Update(entity);
            await _repo.SaveChangesAsync();
            return true;
        }
    }
}
