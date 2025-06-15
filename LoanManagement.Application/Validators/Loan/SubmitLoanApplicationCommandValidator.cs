using FluentValidation;
using LoanManagement.Application.Commands.Loan;

namespace LoanManagement.Application.Validators.Loan;

public class SubmitLoanApplicationCommandValidator : AbstractValidator<SubmitLoanApplicationCommand>
{
    public SubmitLoanApplicationCommandValidator()
    {
        RuleFor(x => x.LoanApplicationId)
            .GreaterThan(0).WithMessage("სესხის განაცხადის ID აუცილებელია");

        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("მომხმარებლის ID აუცილებელია");
    }
}