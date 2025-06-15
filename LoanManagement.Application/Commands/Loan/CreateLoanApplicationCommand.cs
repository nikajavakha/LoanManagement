using LoanManagement.Application.DTOs.Loan;
using LoanManagement.Domain.Enums;
using MediatR;

namespace LoanManagement.Application.Commands.Loan;

public record CreateLoanApplicationCommand(
    int UserId,
    LoanType LoanType,
    decimal Amount,
    Currency Currency,
    int PeriodInMonths) : IRequest<LoanApplicationDto>;