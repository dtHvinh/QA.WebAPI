﻿using FluentValidation;
using WebAPI.Dto;
using WebAPI.Utilities.Contract;

namespace WebAPI.Filters.Validation.Validators;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator(IValidationRuleProvider ruleProvider)
    {
        RuleFor(e => e.Email).EmailAddress();

        ruleProvider.GetPasswordRule(RuleFor(e => e.Password));
    }
}
