using LoanManagement.Application.Commands.Loan;
using LoanManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Handlers.Loan;

public class MoveToPendingApplicationCommandHandler : IRequestHandler<MoveToPendingApplicationCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public MoveToPendingApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(MoveToPendingApplicationCommand request, CancellationToken cancellationToken)
    {
        var loanApplication = await _context.LoanApplications
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (loanApplication == null)
        {
            throw new InvalidOperationException("Loan application not found");
        }

        loanApplication.MoveToPending();
        
        await _context.SaveChangesAsync(cancellationToken);
        
        return true;
    }
}