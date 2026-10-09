
using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class WorkflowInstanceLogRepository : IWorkflowInstanceLogRepository
    {
        private readonly AppDbContext _context;

        public WorkflowInstanceLogRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkflowInstanceLog>> GetByInstanceIdAsync(
            int instanceId)
        {
            return await _context.WorkflowInstanceLogs
                .Include(l => l.StageFrom)
                .Include(l => l.StageTo)
                .Include(l => l.Action)
                .Include(l => l.User)
                .Where(l => l.WorkflowInstId == instanceId)
                .OrderBy(l => l.ActionDate)
                .ToListAsync();
        }

        public async Task AddAsync(WorkflowInstanceLog log)
        {
            if (log.ActionDate == null)
            {
                log.ActionDate = DateTime.UtcNow;
            }

            await _context.WorkflowInstanceLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }
    }
}


