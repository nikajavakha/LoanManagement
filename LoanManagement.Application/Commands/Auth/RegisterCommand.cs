using LoanManagement.Application.DTOs.Auth;
using MediatR;

namespace LoanManagement.Application.Commands.Auth;

public record RegisterCommand(
    string PersonalNumber, 
    string Password, 
    string FirstName, 
    string LastName,
    string PhoneNumber,
    DateTime DateOfBirth) : IRequest<AuthResponse>;