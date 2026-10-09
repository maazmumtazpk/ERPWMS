using ERPWMS.Domain.Interfaces;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

using WeeklyOffEntity = ERPWMS.Database.Models.WeeklyOff;

namespace ERPWMS.Application.Commands.WeeklyOff
{
    public class SetWeeklyOffCommandHandler : IRequestHandler<SetWeeklyOffCommand, int>
    {
        private readonly IWeeklyOffRepository _repo;
        public SetWeeklyOffCommandHandler(IWeeklyOffRepository repo) { _repo = repo; }

        public async Task<int> Handle(SetWeeklyOffCommand request, CancellationToken ct)
        {
            // Close current active weekly off
            var current = await _repo.GetActiveByEmployeeAsync(request.EmployeeId);
            if (current != null)
            {
                current.EffectiveTo = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
                current.IsActive = false;
                current.UpdatedBy = request.CurrentUserId.ToString();
                current.UpdatedDate = DateTime.UtcNow;
                _repo.Update(current);
            }

            var entity = new WeeklyOffEntity
            {
                EmployeeId = request.EmployeeId,
                DayOfWeek = request.DayOfWeek,
                EffectiveFrom = DateOnly.FromDateTime(DateTime.Today),
                Remarks = request.Remarks,
                IsActive = true,
                IsDeleted = false,
                CreatedBy = request.CurrentUserId.ToString(),
                CreatedDate = DateTime.UtcNow
            };
            await _repo.AddAsync(entity);
            await _repo.SaveChangesAsync();
            return entity.WeeklyOffId;
        }
    }
}
