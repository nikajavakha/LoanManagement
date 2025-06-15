using LoanManagement.Application.DTOs.Auth;
using MediatR;

namespace LoanManagement.Application.Commands.Auth;

public record LoginCommand(string PersonalNumber, string Password) : IRequest<AuthResponse>;