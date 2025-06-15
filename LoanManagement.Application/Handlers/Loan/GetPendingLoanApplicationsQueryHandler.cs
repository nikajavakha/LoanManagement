using AutoMapper;
using LoanManagement.Application.DTOs.Loan;
using LoanManagement.Application.Interfaces;
using LoanManagement.Application.Queries.Loan;
using LoanManagement.Domain.Entities;
using LoanManagement.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Handlers.Loan;

public class GetPendingLoanApplicationsQueryHandler: IRequestHandler<GetPendingLoanApplicationsQuery, IEnumerable<LoanApplicationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    
    public GetPendingLoanApplicationsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LoanApplicationDto>> Handle(GetPendingLoanApplicationsQuery request, CancellationToken cancellationToken)
    {
        var applications = await _context.LoanApplications
            .Include(l => l.User)
            .Include(l => l.Approver)
            .OrderByDescending(l => l.CreatedAt)
            .Where(l => l.Status == LoanStatus.Pending)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<LoanApplicationDto>>(applications);
    }
}