using AutoMapper;
using LoanManagement.Application.Commands.Loan;
using LoanManagement.Application.DTOs.Loan;
using LoanManagement.Application.Interfaces;
using LoanManagement.Domain.Entities;
using LoanManagement.Domain.Enums;
using MediatR;

namespace LoanManagement.Application.Handlers.Loan;

public class CreateLoanApplicationCommandHandler : IRequestHandler<CreateLoanApplicationCommand, LoanApplicationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateLoanApplicationCommandHandler(
        IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<LoanApplicationDto> Handle(CreateLoanApplicationCommand request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var loanApplication = new LoanApplication
        {
            UserId = request.UserId,
            LoanType = request.LoanType,
            Amount = request.Amount,
            Currency = request.Currency,
            PeriodInMonths = request.PeriodInMonths,
            Status = LoanStatus.InProcess,
            CreatedAt = now,
            UpdatedAt = now
        };

        _context.LoanApplications.Add(loanApplication);
        await _context.SaveChangesAsync(cancellationToken);

        return _mapper.Map<LoanApplicationDto>(loanApplication);
    }
}