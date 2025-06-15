using LoanManagement.Application.DTOs.Loan;
using MediatR;

namespace LoanManagement.Application.Queries.Loan;

public record GetUserLoanApplicationsQuery(int UserId) : IRequest<IEnumerable<LoanApplicationDto>>;