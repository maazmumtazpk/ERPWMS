
using ERPWMS.Domain.Interfaces;
using ERPWMS.Domain.Services;
using ERPWMS.Domain.Services.Workflow;
using ERPWMS.Database.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERPWMS.Infrastructure.Services
{
    public class WorkflowEngineService(
        IWorkflowDefinitionService definitionService,
        IWorkflowInstanceService instanceService,
        IWorkflowStageService stageService,
        IWorkflowTransitionService transitionService,
        IWorkflowAssignmentService assignmentService,
        IWorkflowInstanceLogService logService,
        ILogger<WorkflowEngineService> logger) : IWorkflowEngineService
    {
        private readonly IWorkflowDefinitionService _definitionService = definitionService;
        private readonly IWorkflowInstanceService _instanceService = instanceService;
        private readonly IWorkflowStageService _stageService = stageService;
        private readonly IWorkflowTransitionService _transitionService = transitionService;
        private readonly IWorkflowAssignmentService _assignmentService = assignmentService;
        private readonly IWorkflowInstanceLogService _logService = logService;
        private readonly ILogger<WorkflowEngineService> _logger = logger;

        // =========================================================
        // START WORKFLOW
        // =========================================================

        public async Task<WorkflowInstance> StartWorkflowAsync(
            int definitionId,
            int entityId,
            string entityType,
            int initiatedBy)
        {
            try
            {
                _logger.LogInformation(
                    "Starting workflow {DefinitionId} for {EntityType}/{EntityId}",
                    definitionId,
                    entityType,
                    entityId);

                var definition =
                    await _definitionService.GetByIdAsync(definitionId);

                if (definition == null)
                {
                    throw new InvalidOperationException(
                        $"Workflow definition {definitionId} not found.");
                }

                var stages =
                    await _stageService.GetByDefinitionIdAsync(definitionId);

                var firstStage = stages
                    .OrderBy(s => s.StageOrder)
                    .FirstOrDefault();

                if (firstStage == null)
                {
                    throw new InvalidOperationException(
                        $"No stages found for workflow definition {definitionId}.");
                }

                var instance = new WorkflowInstance
                {
                    WorkflowDefId = definitionId,
                    RecordId = entityId,
                    RecordType = entityType,
                    CurrentStageId = firstStage.StageId,
                    Status = "InProgress",
                    CreatedBy = initiatedBy.ToString(),
                    CreatedDate = DateTime.UtcNow,
                    StartedDate = DateTime.UtcNow
                };

                await _instanceService.CreateInstanceAsync(instance);

                await CreateAssignmentsForStageAsync(
                    definitionId,
                    instance.WorkflowInstId,
                    firstStage.StageId);

                await LogWorkflowActionAsync(
                    instance.WorkflowInstId,
                    null,
                    firstStage.StageId,
                    0,
                    initiatedBy,
                    "Start");

                return instance;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error starting workflow {DefinitionId}",
                    definitionId);

                throw;
            }
        }

        // =========================================================
        // CURRENT STAGE
        // =========================================================

        public async Task<WorkflowStage?> GetCurrentStageAsync(
            int instanceId)
        {
            var instance =
                await _instanceService.GetInstanceByIdAsync(instanceId);

            if (instance == null)
                return null;

            return await _stageService.GetByIdAsync(
                instance.CurrentStageId);
        }

        // =========================================================
        // AVAILABLE TRANSITIONS
        // =========================================================

        public async Task<IEnumerable<WorkflowTransition>>
            GetAvailableTransitionsAsync(
                int instanceId,
                int userId)
        {
            var instance =
                await _instanceService.GetInstanceByIdAsync(instanceId);

            if (instance == null)
                return Enumerable.Empty<WorkflowTransition>();

            if (instance.Status != "InProgress")
                return Enumerable.Empty<WorkflowTransition>();

            var transitions =
                await _transitionService
                    .GetTransitionsBySourceStageIdAsync(
                        instance.CurrentStageId);

            var result = new List<WorkflowTransition>();

            foreach (var transition in transitions)
            {
                if (transition.IsAllowed == false)
                    continue;

                var hasPermission =
                    await HasUserPermissionForTransitionAsync(
                        instanceId,
                        instance.CurrentStageId,
                        userId,
                        transition);

                if (hasPermission)
                    result.Add(transition);
            }

            return result;
        }

        // =========================================================
        // EXECUTE TRANSITION
        // =========================================================

        public async Task<WorkflowInstance> ExecuteTransitionAsync(
            int instanceId,
            int transitionId,
            int userId,
            string? comment = null)
        {
            try
            {
                var instance =
                    await _instanceService.GetInstanceByIdAsync(instanceId);

                if (instance == null)
                {
                    throw new InvalidOperationException(
                        $"Workflow instance {instanceId} not found.");
                }

                if (instance.Status != "InProgress")
                {
                    throw new InvalidOperationException(
                        $"Workflow instance {instanceId} is not active.");
                }

                var transition =
                    await _transitionService.GetTransitionByIdAsync(
                        transitionId);

                if (transition == null)
                {
                    throw new InvalidOperationException(
                        $"Transition {transitionId} not found.");
                }

                if (transition.StageFromId != instance.CurrentStageId)
                {
                    throw new InvalidOperationException(
                        $"Transition {transitionId} is not available from the current stage.");
                }

                if (transition.IsAllowed == false)
                {
                    throw new InvalidOperationException(
                        $"Transition {transitionId} is not allowed.");
                }

                var hasPermission =
                    await HasUserPermissionForTransitionAsync(
                        instanceId,
                        instance.CurrentStageId,
                        userId,
                        transition);

                if (!hasPermission)
                {
                    throw new UnauthorizedAccessException(
                        $"User {userId} does not have permission for transition {transitionId}.");
                }

                var conditionsMet =
                    await CheckTransitionConditionsAsync(
                        instanceId,
                        transition);

                if (!conditionsMet)
                {
                    throw new InvalidOperationException(
                        $"Transition conditions are not satisfied.");
                }

                var currentStageId =
                    instance.CurrentStageId;

                var targetStageId =
                    transition.StageToId;

                instance.CurrentStageId = targetStageId;
                instance.UpdatedDate = DateTime.UtcNow;
                instance.UpdatedBy = userId.ToString();

                var targetStage =
                    await _stageService.GetByIdAsync(targetStageId);

                if (targetStage?.IsEnd == true)
                {
                    instance.Status = "Completed";
                    instance.CompletedDate = DateTime.UtcNow;

                    await CancelAssignmentsForInstanceAsync(
                        instanceId);
                }
                else
                {
                    instance.Status = "InProgress";

                    await CreateAssignmentsForStageAsync(
                        instance.WorkflowDefId,
                        instance.WorkflowInstId,
                        targetStageId);

                    await CancelAssignmentsForStageAsync(
                        instanceId,
                        currentStageId);
                }

                await _instanceService.UpdateInstanceAsync(
                    instance);

                await LogWorkflowActionAsync(
                    instanceId,
                    currentStageId,
                    targetStageId,
                    transition.ActionId,
                    userId,
                    "Execute",
                    comment);

                return instance;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error executing transition {TransitionId} for instance {InstanceId}",
                    transitionId,
                    instanceId);

                throw;
            }
        }

        // =========================================================
        // COMPLETE WORKFLOW
        // =========================================================

        public async Task<WorkflowInstance> CompleteWorkflowAsync(
            int instanceId,
            int userId,
            string? comment = null)
        {
            var instance =
                await _instanceService.GetInstanceByIdAsync(instanceId);

            if (instance == null)
            {
                throw new InvalidOperationException(
                    $"Workflow instance {instanceId} not found.");
            }

            if (instance.Status == "Completed")
            {
                throw new InvalidOperationException(
                    $"Workflow {instanceId} is already completed.");
            }

            var stage =
                await _stageService.GetByIdAsync(
                    instance.CurrentStageId);

            if (stage?.IsEnd != true)
            {
                throw new InvalidOperationException(
                    $"Cannot complete workflow {instanceId} because the current stage is not final.");
            }

            instance.Status = "Completed";
            instance.CompletedDate = DateTime.UtcNow;
            instance.UpdatedDate = DateTime.UtcNow;
            instance.UpdatedBy = userId.ToString();

            await _instanceService.UpdateInstanceAsync(
                instance);

            await CancelAssignmentsForInstanceAsync(
                instanceId);

            await LogWorkflowActionAsync(
                instanceId,
                instance.CurrentStageId,
                instance.CurrentStageId,
                0,
                userId,
                "Complete",
                comment);

            return instance;
        }

        // =========================================================
        // REJECT WORKFLOW
        // =========================================================

        public async Task<WorkflowInstance> RejectWorkflowAsync(
            int instanceId,
            int userId,
            string? comment = null)
        {
            var instance =
                await _instanceService.GetInstanceByIdAsync(instanceId);

            if (instance == null)
            {
                throw new InvalidOperationException(
                    $"Workflow instance {instanceId} not found.");
            }

            instance.Status = "Rejected";
            instance.CompletedDate = DateTime.UtcNow;
            instance.UpdatedDate = DateTime.UtcNow;
            instance.UpdatedBy = userId.ToString();

            await _instanceService.UpdateInstanceAsync(
                instance);

            await CancelAssignmentsForInstanceAsync(
                instanceId);

            await LogWorkflowActionAsync(
                instanceId,
                instance.CurrentStageId,
                instance.CurrentStageId,
                0,
                userId,
                "Reject",
                comment);

            return instance;
        }

        // =========================================================
        // CANCEL WORKFLOW
        // =========================================================

        public async Task<WorkflowInstance> CancelWorkflowAsync(
            int instanceId,
            int userId,
            string? comment = null)
        {
            var instance =
                await _instanceService.GetInstanceByIdAsync(instanceId);

            if (instance == null)
            {
                throw new InvalidOperationException(
                    $"Workflow instance {instanceId} not found.");
            }

            instance.Status = "Cancelled";
            instance.CompletedDate = DateTime.UtcNow;
            instance.UpdatedDate = DateTime.UtcNow;
            instance.UpdatedBy = userId.ToString();

            await _instanceService.UpdateInstanceAsync(
                instance);

            await CancelAssignmentsForInstanceAsync(
                instanceId);

            await LogWorkflowActionAsync(
                instanceId,
                instance.CurrentStageId,
                instance.CurrentStageId,
                0,
                userId,
                "Cancel",
                comment);

            return instance;
        }

        // =========================================================
        // WORKFLOW HISTORY
        // =========================================================

        public async Task<IEnumerable<WorkflowInstanceLog>>
            GetWorkflowHistoryAsync(int instanceId)
        {
            return await _logService.GetByInstanceIdAsync(
                instanceId);
        }

        // =========================================================
        // ASSIGNMENTS
        // =========================================================

        public async Task<IEnumerable<WorkflowAssignment>>
            GetAssignmentsAsync(int instanceId)
        {
            return await _assignmentService
                .GetAssignmentsByInstanceAsync(instanceId);
        }

        // =========================================================
        // PRIVATE: PERMISSION
        // =========================================================

        private async Task<bool>
            HasUserPermissionForTransitionAsync(
                int instanceId,
                int stageId,
                int userId,
                WorkflowTransition transition)
        {
            try
            {
                var assignments =
                    await _assignmentService
                        .GetAssignmentsByInstanceAndStageAsync(
                            instanceId,
                            stageId);

                foreach (var assignment in assignments)
                {
                    if (assignment.IsActive != true)
                        continue;

                    if (assignment.UserId.HasValue &&
                        assignment.UserId.Value == userId)
                    {
                        // The WorkflowTransition model contains
                        // ActionId, not ActionCode.
                        //
                        // Until WorkflowAction's actual code/name
                        // property is confirmed, an active direct
                        // user assignment is accepted here.

                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error checking transition permission.");

                return false;
            }
        }

        // =========================================================
        // PRIVATE: CONDITIONS
        // =========================================================

        private async Task<bool>
            CheckTransitionConditionsAsync(
                int instanceId,
                WorkflowTransition transition)
        {
            // Condition engine can be implemented once
            // WorkflowCondition structure is confirmed.

            await Task.CompletedTask;

            return true;
        }

        // =========================================================
        // PRIVATE: CREATE ASSIGNMENTS
        // =========================================================

        private async Task CreateAssignmentsForStageAsync(
            int definitionId,
            int instanceId,
            int stageId)
        {
            var assignments =
                await _assignmentService
                    .GetAssignmentsByDefinitionAndStageAsync(
                        definitionId,
                        stageId);

            foreach (var assignment in assignments)
            {
                var instanceAssignment =
                    new WorkflowAssignment
                    {
                        WorkflowDefId = definitionId,
                        StageId = stageId,
                        UserId = assignment.UserId,
                        RoleId = assignment.RoleId,
                        CanApprove = assignment.CanApprove,
                        CanReject = assignment.CanReject,
                        CanHold = assignment.CanHold,
                        CanSendBack = assignment.CanSendBack,
                        IsActive = true,
                        CreatedBy = assignment.CreatedBy,
                        CreatedDate = DateTime.UtcNow
                    };

                await _assignmentService.CreateAssignmentAsync(
                    instanceAssignment);
            }

            _logger.LogInformation(
                "Created {Count} assignments for stage {StageId}",
                assignments.Count(),
                stageId);
        }

        // =========================================================
        // PRIVATE: CANCEL ALL INSTANCE ASSIGNMENTS
        // =========================================================

        private async Task CancelAssignmentsForInstanceAsync(
            int instanceId)
        {
            var assignments =
                await _assignmentService
                    .GetAssignmentsByInstanceAsync(instanceId);

            foreach (var assignment in assignments)
            {
                assignment.IsActive = false;
                assignment.UpdatedDate = DateTime.UtcNow;

                await _assignmentService.UpdateAssignmentAsync(
                    assignment);
            }

            _logger.LogInformation(
                "Cancelled {Count} assignments for instance {InstanceId}",
                assignments.Count(),
                instanceId);
        }

        // =========================================================
        // PRIVATE: CANCEL STAGE ASSIGNMENTS
        // =========================================================

        private async Task CancelAssignmentsForStageAsync(
            int instanceId,
            int stageId)
        {
            var assignments =
                await _assignmentService
                    .GetAssignmentsByInstanceAndStageAsync(
                        instanceId,
                        stageId);

            foreach (var assignment in assignments)
            {
                assignment.IsActive = false;
                assignment.UpdatedDate = DateTime.UtcNow;

                await _assignmentService.UpdateAssignmentAsync(
                    assignment);
            }

            _logger.LogInformation(
                "Cancelled {Count} assignments for stage {StageId}",
                assignments.Count(),
                stageId);
        }

        // =========================================================
        // PRIVATE: LOG WORKFLOW ACTION
        // =========================================================

        private async Task LogWorkflowActionAsync(
            int instanceId,
            int? stageFromId,
            int stageToId,
            int actionId,
            int userId,
            string actionName,
            string? comment = null)
        {
            try
            {
                var log = new WorkflowInstanceLog
                {
                    WorkflowInstId = instanceId,
                    StageFromId = stageFromId,
                    StageToId = stageToId,
                    ActionId = actionId,
                    UserId = userId,
                    Comments = comment,
                    ActionDate = DateTime.UtcNow
                };

                await _logService.AddAsync(log);

                _logger.LogInformation(
                    "Logged workflow action {ActionName} for instance {InstanceId}",
                    actionName,
                    instanceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error logging workflow action for instance {InstanceId}",
                    instanceId);
            }
        }
    }
}


