using LoanManagement.Domain.Enums;
using MediatR;

namespace LoanManagement.Application.Commands.Loan;

public record UpdateLoanApplicationCommand(
    int UserId,
    int LoanApplicationId,
    LoanType LoanType,
    decimal Amount,
    Currency Currency,
    int PeriodInMonths) : IRequest<bool>;