using FluentValidation;
using NexaERP.BLL.DTOs.TaskComment;

namespace NexaERP.BLL.Validators.TaskComment;

public sealed class UpdateTaskCommentDtoValidator
    : AbstractValidator<UpdateTaskCommentDto>
{
    public UpdateTaskCommentDtoValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
