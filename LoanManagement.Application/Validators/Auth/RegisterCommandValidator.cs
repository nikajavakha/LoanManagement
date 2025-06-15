using FluentValidation;
using LoanManagement.Application.Commands.Auth;

namespace LoanManagement.Application.Validators.Auth;

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.PersonalNumber)
            .NotEmpty().WithMessage("პირადი ნომერი აუცილებელია")
            .Length(11).WithMessage("პირადი ნომერი უნდა იყოს 11 რიცხვი")
            .Matches(@"^\d{11}$").WithMessage("პირადი ნომერი უნდა შეიცავდეს მხოლოდ რიცხვებს");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("ტელეფონის ნომერი აუცილებელია")
            .Matches(@"^(\+995|995)?(5\d{8}|32\d{6})$|^5\d{8}$|^2\d{7}$")
            .WithMessage("ტელეფონის ნომერი არასწორია");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("პაროლი აუცილებელია")
            .MinimumLength(6).WithMessage("პაროლი უნდა იყოს მინიმუმ 6 სიმბოლო");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("სახელი აუცილებელია")
            .Length(2, 50).WithMessage("სახელი უნდა იყოს 2-50 სიმბოლო");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("გვარი აუცილებელია")
            .Length(2, 50).WithMessage("გვარი უნდა იყოს 2-50 სიმბოლო");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("დაბადების თარიღი აუცილებელია")
            .Must(BeValidAge).WithMessage("ასაკი უნდა იყოს 18-დან 100 წლამდე");
    }

    private static bool BeValidAge(DateTime dateOfBirth)
    {
        var age = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth.Date > DateTime.Today.AddYears(-age)) age--;
        return age >= 18 && age <= 100;
    }
}