using FluentValidation;
using Tweetbook.Contracts.V1.Requests;

namespace Tweetbook.Validators
{
    public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
    {
        public CreatePostRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .Matches("^[a-zA-Z0-9 ]*$");

            RuleFor(x => x.Tags.Count)
                .NotNull()
                .GreaterThan(0)
                .WithMessage("Post must contain at least one tag.");

            RuleFor(x => x.Tags)
                .NotEmpty()
                .Must(x => x.All(tag => !string.IsNullOrWhiteSpace(tag.Name)))
                .WithMessage("Tag name must not be empty.");
        }
    }
}
