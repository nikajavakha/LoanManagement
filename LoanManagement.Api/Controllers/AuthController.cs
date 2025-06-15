using LoanManagement.Application.Commands.Auth;
using LoanManagement.Application.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        var command = new RegisterCommand(request.PersonalNumber, request.Password, request.FirstName,
            request.LastName, request.PhoneNumber, request.DateOfBirth);
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        var command = new LoginCommand(request.PersonalNumber, request.Password);
        var response = await _mediator.Send(command);
        return Ok(response);
    }
}