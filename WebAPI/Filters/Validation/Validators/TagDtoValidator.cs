﻿using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Filters.Validation.Validators;

public class TagDtoValidator : AbstractValidator<CreateTagDto>
{
    public TagDtoValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty()
            .WithMessage("Name too short")
            .MaximumLength(50)
            .WithMessage("Name too long");

        RuleFor(e => e.Description)
            .MaximumLength(1000)
            .WithMessage("Description too long");
    }
}
