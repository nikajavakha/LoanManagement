using LoanManagement.Application.DTOs.Loan;
using MediatR;

namespace LoanManagement.Application.Queries.Loan;

public record GetLoanApplicationByIdQuery(int Id, int UserId, string UserRole) : IRequest<LoanApplicationDto?>;