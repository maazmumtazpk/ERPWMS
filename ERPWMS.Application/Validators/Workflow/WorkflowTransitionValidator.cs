using ERPWMS.Application.DTOs.Workflow;
using FluentValidation;

namespace ERPWMS.Application.Validators.Workflow
{
    public class CreateWorkflowTransitionValidator : AbstractValidator<CreateWorkflowTransitionRequest>
    {
        public CreateWorkflowTransitionValidator()
        {
            RuleFor(x => x.WorkflowDefinitionId)
                .NotEmpty().WithMessage("Valid Workflow Definition is required.");

            RuleFor(x => x.SourceStageId)
                .NotEmpty().WithMessage("Source Stage is required.");

            RuleFor(x => x.DestinationStageId)
                .NotEmpty().WithMessage("Destination Stage is required.");

            RuleFor(x => x.ActionId)
                .NotEmpty().WithMessage("Action is required.");
        }
    }

    public class UpdateWorkflowTransitionValidator : AbstractValidator<UpdateWorkflowTransitionRequest>
    {
        public UpdateWorkflowTransitionValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Invalid transition ID.");

            RuleFor(x => x.WorkflowDefinitionId)
                .NotEmpty().WithMessage("Valid Workflow Definition is required.");

            RuleFor(x => x.SourceStageId)
                .NotEmpty().WithMessage("Source Stage is required.");

            RuleFor(x => x.DestinationStageId)
                .NotEmpty().WithMessage("Destination Stage is required.");

            RuleFor(x => x.ActionId)
                .NotEmpty().WithMessage("Action is required.");
        }
    }
}