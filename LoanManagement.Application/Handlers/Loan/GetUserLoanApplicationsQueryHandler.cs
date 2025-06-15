using AutoMapper;
using LoanManagement.Application.DTOs.Loan;
using LoanManagement.Application.Interfaces;
using LoanManagement.Application.Queries.Loan;
using LoanManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Handlers.Loan;

public class GetUserLoanApplicationsQueryHandler : IRequestHandler<GetUserLoanApplicationsQuery, IEnumerable<LoanApplicationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetUserLoanApplicationsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LoanApplicationDto>> Handle(GetUserLoanApplicationsQuery request, CancellationToken cancellationToken)
    {
        var applications = await _context.LoanApplications
            .Include(l => l.User)
            .Include(l => l.Approver)
            .OrderByDescending(l => l.CreatedAt)
            .Where(l => l.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        return _mapper.Map<IEnumerable<LoanApplicationDto>>(applications);
    }
}