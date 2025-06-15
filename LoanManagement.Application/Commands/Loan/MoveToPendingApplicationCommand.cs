using MediatR;

namespace LoanManagement.Application.Commands.Loan;

public record MoveToPendingApplicationCommand(int Id) : IRequest<bool>;