using LoanManagement.Application.Commands.Loan;
using LoanManagement.Application.Interfaces;
using LoanManagement.Application.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Handlers.Loan;

public class SubmitLoanApplicationCommandHandler : IRequestHandler<SubmitLoanApplicationCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ILoanApplicationQueueService _queueService;

    public SubmitLoanApplicationCommandHandler(
        IApplicationDbContext context,
        ILoanApplicationQueueService queueService)
    {
        _context = context;
        _queueService = queueService;
    }

    public async Task<bool> Handle(SubmitLoanApplicationCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var loanApplication = await _context.LoanApplications
                .FirstOrDefaultAsync(l => l.Id == request.LoanApplicationId && l.UserId == request.UserId,
                    cancellationToken);

            if (loanApplication == null)
            {
                throw new InvalidOperationException("Loan application not found");
            }

            loanApplication.Submit();
            
            await _context.SaveChangesAsync(cancellationToken);
            
            await _queueService.PublishLoanApplicationAsync(request.LoanApplicationId);
            
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        return true;
    }
}