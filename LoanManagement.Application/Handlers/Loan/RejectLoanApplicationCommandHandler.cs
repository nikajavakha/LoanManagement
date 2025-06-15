using LoanManagement.Application.Commands.Loan;
using LoanManagement.Application.Interfaces;
using LoanManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Handlers.Loan;

public class RejectLoanApplicationCommandHandler : IRequestHandler<RejectLoanApplicationCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public RejectLoanApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(RejectLoanApplicationCommand request, CancellationToken cancellationToken)
    {
        var loanApplication = await _context.LoanApplications
            .FirstOrDefaultAsync(l => l.Id == request.LoanApplicationId, cancellationToken);

        if (loanApplication == null)
        {
            throw new InvalidOperationException("Loan application not found");
        }

        loanApplication.Reject(request.ApproverId, request.RejectionReason);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}