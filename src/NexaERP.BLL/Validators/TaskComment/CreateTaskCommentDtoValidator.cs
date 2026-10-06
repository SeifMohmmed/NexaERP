using FluentValidation;
using NexaERP.BLL.DTOs.TaskComment;

namespace NexaERP.BLL.Validators.TaskComment;

public sealed class CreateTaskCommentDtoValidator
    : AbstractValidator<CreateTaskCommentDto>
{
    public CreateTaskCommentDtoValidator()
    {
        RuleFor(x => x.Body)
            .NotEmpty()
            .MaximumLength(2000);
    }
}
