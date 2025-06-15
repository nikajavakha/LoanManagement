using AutoMapper;
using LoanManagement.Application.DTOs.Loan;
using LoanManagement.Application.Interfaces;
using LoanManagement.Application.Queries.Loan;
using LoanManagement.Domain.Constants;
using LoanManagement.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LoanManagement.Application.Handlers.Loan;

public class GetLoanApplicationByIdQueryHandler : IRequestHandler<GetLoanApplicationByIdQuery, LoanApplicationDto?>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetLoanApplicationByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<LoanApplicationDto?> Handle(GetLoanApplicationByIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.LoanApplications
            .Include(l => l.User)
            .Include(l => l.Approver)
            .AsQueryable();

        if (request.UserRole != Roles.Approver)
        {
            query = query.Where(l => l.UserId == request.UserId);
        }

        var application = await query
            .FirstOrDefaultAsync(l => l.Id == request.Id, cancellationToken);

        if (application == null)
            return null;

        return _mapper.Map<LoanApplicationDto>(application);
    }
}