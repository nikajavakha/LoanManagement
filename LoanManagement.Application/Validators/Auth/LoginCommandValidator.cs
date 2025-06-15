using FluentValidation;
using LoanManagement.Application.Commands.Auth;

namespace LoanManagement.Application.Validators.Auth;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.PersonalNumber)
            .NotEmpty().WithMessage("პირადი ნომერი აუცილებელია")
            .Length(11).WithMessage("პირადი ნომერი უნდა იყოს 11 რიცხვი")
            .Matches(@"^\d{11}$").WithMessage("პირადი ნომერი უნდა შეიცავდეს მხოლოდ რიცხვებს");
        
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("პაროლი აუცილებელია")
            .MinimumLength(6).WithMessage("პაროლი უნდა იყოს მინიმუმ 6 სიმბოლო");
    }
}