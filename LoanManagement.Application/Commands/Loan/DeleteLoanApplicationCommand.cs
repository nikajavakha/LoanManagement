using MediatR;

namespace LoanManagement.Application.Commands.Loan;

public record DeleteLoanApplicationCommand(
    int LoanApplicationId,
    int UserId
) : IRequest<bool>;