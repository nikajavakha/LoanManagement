using MediatR;

namespace LoanManagement.Application.Commands.Loan;

public record ApproveLoanApplicationCommand(
    int LoanApplicationId,
    int ApproverId) : IRequest<bool>;