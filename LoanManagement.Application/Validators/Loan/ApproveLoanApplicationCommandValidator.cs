using FluentValidation;
using LoanManagement.Application.Commands.Loan;

namespace LoanManagement.Application.Validators.Loan;

public class ApproveLoanApplicationCommandValidator : AbstractValidator<ApproveLoanApplicationCommand>
{
    public ApproveLoanApplicationCommandValidator()
    {
        RuleFor(x => x.LoanApplicationId)
            .GreaterThan(0).WithMessage("სესხის განაცხადის ID აუცილებელია");

        RuleFor(x => x.ApproverId)
            .GreaterThan(0).WithMessage("დამამტკიცებლის ID აუცილებელია");
    }
}