using FluentValidation;
using Gigs.Domain.Models.Commands;

namespace Gigs.Domain.Models.Validators
{
    public class UpdateGigCommandValidator : AbstractValidator<UpdateGigCommand>
    {
        public UpdateGigCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Valid Gig ID is required");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required")
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required")
                .MaximumLength(100).WithMessage("Category cannot exceed 100 characters");

            RuleFor(x => x.DeliveryDays)
                .GreaterThan(0).WithMessage("Delivery days must be greater than 0")
                .LessThanOrEqualTo(365).WithMessage("Delivery days cannot exceed 365 days");
        }
    }
}