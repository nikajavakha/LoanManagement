using LoanManagement.Application.Commands.Loan;
using LoanManagement.Application.Interfaces;
using LoanManagement.Domain;
using LoanManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Handlers.Loan;

public class ApproveLoanApplicationCommandHandler : IRequestHandler<ApproveLoanApplicationCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public ApproveLoanApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(ApproveLoanApplicationCommand request, CancellationToken cancellationToken)
    {
        var loanApplication = await _context.LoanApplications
            .FirstOrDefaultAsync(l => l.Id == request.LoanApplicationId, cancellationToken);

        if (loanApplication == null)
        {
            throw new InvalidOperationException("Loan application not found");
        }
        
        loanApplication.Approve(request.ApproverId);
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}