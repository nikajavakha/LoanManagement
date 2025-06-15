using MediatR;

namespace LoanManagement.Application.Commands.Loan;

public record RejectLoanApplicationCommand( 
    int LoanApplicationId,
    int ApproverId,
    string RejectionReason) : IRequest<bool>;