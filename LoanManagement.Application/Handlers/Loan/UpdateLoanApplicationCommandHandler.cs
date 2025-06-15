using LoanManagement.Application.Commands.Loan;
using LoanManagement.Application.DTOs.Loan;
using LoanManagement.Application.Interfaces;
using LoanManagement.Domain.Entities;
using LoanManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Handlers.Loan;

public class UpdateLoanApplicationCommandHandler : IRequestHandler<UpdateLoanApplicationCommand, bool>
{
    private readonly IApplicationDbContext _context;

    public UpdateLoanApplicationCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateLoanApplicationCommand request, CancellationToken cancellationToken)
    {
        var loanApplication = await _context.LoanApplications
            .Include(l => l.User)
            .Include(l => l.Approver)
            .FirstOrDefaultAsync(l =>
                l.Id == request.LoanApplicationId && 
                l.UserId == request.UserId, cancellationToken);

        if (loanApplication == null)
            throw new UnauthorizedAccessException("Loan application not found or access denied");

        loanApplication.Update(
            request.LoanType, 
            request.Amount, 
            request.Currency, 
            request.PeriodInMonths);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}