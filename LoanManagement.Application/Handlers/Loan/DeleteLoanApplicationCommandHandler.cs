using LoanManagement.Application.Commands.Loan;
using LoanManagement.Application.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace LoanManagement.Application.Handlers.Loan;

public class DeleteLoanApplicationCommandHandler : IRequestHandler<DeleteLoanApplicationCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteLoanApplicationCommandHandler> _logger;

    public DeleteLoanApplicationCommandHandler(
        IApplicationDbContext context, 
        ILogger<DeleteLoanApplicationCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteLoanApplicationCommand request, CancellationToken cancellationToken)
    {
        var loanApplication = await _context.LoanApplications
            .FirstOrDefaultAsync(l => 
                l.Id == request.LoanApplicationId && 
                l.UserId == request.UserId, cancellationToken);

        if (loanApplication == null)
        {
            throw new InvalidOperationException("Loan application not found or access denied");
        }
        
        if (!loanApplication.CanBeEdited())
        {
            throw new InvalidOperationException("Cannot delete loan application in current status");
        }

        _context.LoanApplications.Remove(loanApplication);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}