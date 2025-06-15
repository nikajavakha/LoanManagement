using MediatR;

namespace LoanManagement.Application.Commands.Loan;

public record SubmitLoanApplicationCommand(  
    int LoanApplicationId,
    int UserId) : IRequest<bool>;