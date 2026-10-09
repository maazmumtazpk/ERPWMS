using ERPWMS.Domain.Interfaces;
using ERPWMS.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ERPWMS.Database;

namespace ERPWMS.Infrastructure.Repositories
{
    public class WorkflowAssignmentRepository : IWorkflowAssignmentRepository
    {
        private readonly AppDbContext _context;

        public WorkflowAssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetByInstanceIdAsync(int instanceId)
        {
            return await _context.WorkflowAssignments
                .Where(w => w.WorkflowDefId == instanceId && (w.IsActive ?? true))
                .Include(w => w.User)
                .Include(w => w.Role)
                .Include(w => w.Stage)
                .Include(w => w.WorkflowDef)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetByStageIdAsync(int stageId)
        {
            return await _context.WorkflowAssignments
                .Where(w => w.StageId == stageId && (w.IsActive ?? true))
                .Include(w => w.User)
                .Include(w => w.Role)
                .Include(w => w.Stage)
                .Include(w => w.WorkflowDef)
                .ToListAsync();
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetByUserAsync(int userId)
        {
            return await _context.WorkflowAssignments
                .Where(w => w.UserId == userId && (w.IsActive ?? true))
                .Include(w => w.User)
                .Include(w => w.Role)
                .Include(w => w.Stage)
                .Include(w => w.WorkflowDef)
                .ToListAsync();
        }

        public async Task<WorkflowAssignment?> GetAssignmentAsync(int instanceId, int stageId, int? userId, int? roleId)
        {
            // Pehle IQueryable banayein
            var query = _context.WorkflowAssignments
                .Where(w => w.WorkflowDefId == instanceId && w.StageId == stageId && (w.IsActive ?? true));

            // Phir conditions apply karein
            if (userId.HasValue)
            {
                query = query.Where(w => w.UserId == userId);
            }

            if (roleId.HasValue)
            {
                query = query.Where(w => w.RoleId == roleId);
            }

            // Last mein Include karein
            return await query
                .Include(w => w.User)
                .Include(w => w.Role)
                .Include(w => w.Stage)
                .Include(w => w.WorkflowDef)
                .FirstOrDefaultAsync();
        }

        public async Task AddAsync(WorkflowAssignment assignment)
        {
            assignment.CreatedDate = DateTime.Now;
            assignment.IsActive = true;

            await _context.WorkflowAssignments.AddAsync(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WorkflowAssignment assignment)
        {
            assignment.UpdatedDate = DateTime.Now;

            _context.WorkflowAssignments.Update(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var assignment = await _context.WorkflowAssignments.FindAsync(id);
            if (assignment == null)
                return false;

            // Soft delete
            assignment.IsActive = false;
            assignment.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        // Extra helper methods
        public async Task<IEnumerable<WorkflowAssignment>> GetByWorkflowDefIdAsync(int workflowDefId)
        {
            return await _context.WorkflowAssignments
                .Where(w => w.WorkflowDefId == workflowDefId && (w.IsActive ?? true))
                .Include(w => w.User)
                .Include(w => w.Role)
                .Include(w => w.Stage)
                .Include(w => w.WorkflowDef)
                .ToListAsync();
        }

        public async Task<WorkflowAssignment?> GetByIdAsync(int assignmentId)
        {
            return await _context.WorkflowAssignments
                .Where(w => w.AssignmentId == assignmentId && (w.IsActive ?? true))
                .Include(w => w.User)
                .Include(w => w.Role)
                .Include(w => w.Stage)
                .Include(w => w.WorkflowDef)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<WorkflowAssignment>> GetByUserWithPermissionsAsync(int userId)
        {
            return await _context.WorkflowAssignments
                .Where(w => w.UserId == userId && (w.IsActive ?? true))
                .Where(w => w.CanApprove == true || w.CanReject == true || w.CanHold == true || w.CanSendBack == true)
                .Include(w => w.User)
                .Include(w => w.Role)
                .Include(w => w.Stage)
                .Include(w => w.WorkflowDef)
                .ToListAsync();
        }

        public async Task<bool> HasUserPermissionAsync(int assignmentId, int userId, string permissionType)
        {
            var assignment = await _context.WorkflowAssignments
                .FirstOrDefaultAsync(w => w.AssignmentId == assignmentId && w.UserId == userId);

            if (assignment == null)
                return false;

            return permissionType switch
            {
                "Approve" => assignment.CanApprove ?? false,
                "Reject" => assignment.CanReject ?? false,
                "Hold" => assignment.CanHold ?? false,
                "SendBack" => assignment.CanSendBack ?? false,
                _ => false
            };
        }
    }
}
