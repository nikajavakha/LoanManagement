using FluentValidation;
using LoanManagement.Application.Commands.Loan;

namespace LoanManagement.Application.Validators.Loan;

public class CreateLoanApplicationCommandValidator : AbstractValidator<CreateLoanApplicationCommand>
{
    public CreateLoanApplicationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("მომხმარებლის ID აუცილებელია");

        RuleFor(x => x.LoanType)
            .IsInEnum().WithMessage("სესხის ტიპი არასწორია");

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("თანხა უნდა იყოს დადებითი")
            .LessThanOrEqualTo(1000000).WithMessage("თანხა არ უნდა აღემატებოდეს 1,000,000-ს");

        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("ვალუტა არასწორია");

        RuleFor(x => x.PeriodInMonths)
            .GreaterThan(0).WithMessage("პერიოდი უნდა იყოს დადებითი")
            .LessThanOrEqualTo(360).WithMessage("პერიოდი არ უნდა აღემატებოდეს 360 თვეს");
    }
}