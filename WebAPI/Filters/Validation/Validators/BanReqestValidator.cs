using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Filters.Validation.Validators;

public class BanReqestValidator : AbstractValidator<BanUserDto>
{
    public BanReqestValidator()
    {
        RuleFor(e => e.Hours)
            .GreaterThanOrEqualTo(0).WithMessage("Hours must be greater than 0")
            .LessThanOrEqualTo(24).WithMessage("Hours must be less than or equal to 24");

        RuleFor(e => e.Minutes)
            .GreaterThanOrEqualTo(0).WithMessage("Minutes must be greater than 0")
            .LessThanOrEqualTo(60).WithMessage("Minutes must be less than or equal to 60");
    }
}
