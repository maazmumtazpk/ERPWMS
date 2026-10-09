using ERPWMS.Domain.Services.Workflow;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Services
{
    public class WorkflowActionService(AppDbContext context) : IWorkflowActionService
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<WorkflowAction>> GetAllActionsAsync(
            bool includeInactive = false)
        {
            var query = _context.WorkflowActions.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(a => a.IsActive == true);
            }

            return await query.ToListAsync();
        }

        public async Task<WorkflowAction?> GetActionByIdAsync(int id)
        {
            return await _context.WorkflowActions
                .FirstOrDefaultAsync(a => a.ActionId == id);
        }
    }
}