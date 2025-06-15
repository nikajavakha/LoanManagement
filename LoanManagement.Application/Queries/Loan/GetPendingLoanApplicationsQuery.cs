using LoanManagement.Application.DTOs.Loan;
using MediatR;

namespace LoanManagement.Application.Queries.Loan;

public record GetPendingLoanApplicationsQuery : IRequest<IEnumerable<LoanApplicationDto>>;