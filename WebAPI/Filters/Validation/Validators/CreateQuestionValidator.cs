﻿using FluentValidation;
using WebAPI.Dto;

namespace WebAPI.Filters.Validation.Validators;

public class CreateQuestionValidator : AbstractValidator<CreateQuestionDto>
{
    public CreateQuestionValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MaximumLength(150).WithMessage("Title too long");

        RuleFor(e => e.Content)
            .MinimumLength(20).WithMessage("Content is too short.");

        RuleFor(e => e.Tags)
            .NotEmpty().WithMessage("Tags are required.")
            .Must(e => e.Count <= 5).WithMessage("Tag limit is 5");
    }
}
