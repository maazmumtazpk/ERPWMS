
using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services.Workflow;
using ERPWMS.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class WorkflowAssignmentService(
        IWorkflowAssignmentRepository repository) : IWorkflowAssignmentService
    {
        private readonly IWorkflowAssignmentRepository _repository = repository;

        // =========================================================
        // GET ASSIGNMENTS BY INSTANCE
        // =========================================================

        public async Task<IEnumerable<WorkflowAssignment>>
            GetAssignmentsByInstanceAsync(int instanceId)
        {
            // WorkflowAssignment does not contain WorkflowInstanceId.
            // Therefore assignments cannot be filtered directly by instance.

            return await Task.FromResult(
                Enumerable.Empty<WorkflowAssignment>());
        }

        // =========================================================
        // GET ASSIGNMENTS BY INSTANCE + STAGE
        // =========================================================

        public async Task<IEnumerable<WorkflowAssignment>>
            GetAssignmentsByInstanceAndStageAsync(
                int instanceId,
                int stageId)
        {
            // WorkflowAssignment does not contain WorkflowInstanceId.
            // We can safely filter by stage only.

            return await _repository.GetByStageIdAsync(stageId);
        }

        // =========================================================
        // GET ASSIGNMENTS BY DEFINITION + STAGE
        // =========================================================

        public async Task<IEnumerable<WorkflowAssignment>>
            GetAssignmentsByDefinitionAndStageAsync(
                int definitionId,
                int stageId)
        {
            // WorkflowAssignment does not expose WorkflowDefId.
            // Therefore definition filtering cannot be performed here.
            // StageId is the available relationship.

            return await _repository.GetByStageIdAsync(stageId);
        }

        // =========================================================
        // GET ASSIGNMENTS BY STAGE
        // =========================================================

        public async Task<IEnumerable<WorkflowAssignment>>
            GetAssignmentsByStageAsync(int stageId)
        {
            return await _repository.GetByStageIdAsync(stageId);
        }

        // =========================================================
        // GET SINGLE ASSIGNMENT
        // =========================================================

        public async Task<WorkflowAssignment?> GetAssignmentAsync(
            int instanceId,
            int stageId,
            int? userId,
            int? roleId)
        {
            // WorkflowAssignment has no WorkflowInstanceId.
            // Repository lookup therefore uses the available
            // stage/user/role information.

            return await _repository.GetAssignmentAsync(
                0,
                stageId,
                userId,
                roleId);
        }

        // =========================================================
        // CREATE
        // =========================================================

        public async Task<WorkflowAssignment>
            CreateAssignmentAsync(
                WorkflowAssignment assignment)
        {
            var existing = await _repository.GetAssignmentAsync(
                0,
                assignment.StageId,
                assignment.UserId,
                assignment.RoleId);

            if (existing != null)
            {
                throw new InvalidOperationException(
                    "Assignment already exists for this user/role and stage.");
            }

            assignment.CreatedDate = DateTime.UtcNow;

            await _repository.AddAsync(assignment);

            return assignment;
        }

        // =========================================================
        // UPDATE
        // =========================================================

        public async Task UpdateAssignmentAsync(
            WorkflowAssignment assignment)
        {
            assignment.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(assignment);
        }

        // =========================================================
        // DELETE
        // =========================================================

        public async Task<bool> DeleteAssignmentAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        // =========================================================
        // ACTION AUTHORIZATION
        // =========================================================

        public async Task<bool> HasAssignmentForActionAsync(
            int instanceId,
            int stageId,
            int userId,
            string actionType)
        {
            var assignments =
                await _repository.GetByStageIdAsync(stageId);

            foreach (var assignment in assignments)
            {
                if (assignment.StageId != stageId)
                    continue;

                if (assignment.IsActive != true)
                    continue;

                if (!assignment.UserId.HasValue ||
                    assignment.UserId.Value != userId)
                    continue;

                var allowed = actionType.ToLowerInvariant() switch
                {
                    "approve" =>
                        assignment.CanApprove == true,

                    "reject" =>
                        assignment.CanReject == true,

                    "hold" =>
                        assignment.CanHold == true,

                    "sendback" or
                    "send-back" or
                    "send_back" =>
                        assignment.CanSendBack == true,

                    _ => false
                };

                if (allowed)
                    return true;
            }

            return false;
        }
    }
}


