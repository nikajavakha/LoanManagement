using FluentValidation;
using LoanManagement.Application.Commands.Loan;

namespace LoanManagement.Application.Validators.Loan;

public class RejectLoanApplicationCommandValidator: AbstractValidator<RejectLoanApplicationCommand>
{
    public RejectLoanApplicationCommandValidator()
    {
        RuleFor(x => x.LoanApplicationId)
            .GreaterThan(0).WithMessage("სესხის განაცხადის ID აუცილებელია");

        RuleFor(x => x.ApproverId)
            .GreaterThan(0).WithMessage("დამამტკიცებლის ID აუცილებელია");

        RuleFor(x => x.RejectionReason)
            .NotEmpty().WithMessage("უარყოფის მიზეზი აუცილებელია")
            .Length(10, 500).WithMessage("უარყოფის მიზეზი უნდა იყოს 10-500 სიმბოლო");
    }
}