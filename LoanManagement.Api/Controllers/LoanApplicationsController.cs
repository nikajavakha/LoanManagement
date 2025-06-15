using System.Security.Claims;
using LoanManagement.Application.Commands.Loan;
using LoanManagement.Application.DTOs.Loan;
using LoanManagement.Application.Queries.Loan;
using LoanManagement.Domain.Constants;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LoanManagement.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LoanApplicationsController : ControllerBase
{
    private readonly IMediator _mediator;

    public LoanApplicationsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LoanApplicationDto>>> GetLoanApplications()
    {
        var applications = await _mediator.Send(new GetUserLoanApplicationsQuery(GetUserId()));
        return Ok(applications);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LoanApplicationDto>> GetLoanApplication(int id)
    {
        var userId = GetUserId();
        var userRole = GetUserRole();

        var application = await _mediator.Send(new GetLoanApplicationByIdQuery(id, userId, userRole));

        if (application == null)
        {
            return NotFound();
        }

        return Ok(application);
    }


    [HttpPost]
    public async Task<ActionResult<LoanApplicationDto>> CreateLoanApplication(
        [FromBody] CreateLoanApplicationRequest request)
    {
        var userId = GetUserId();
        var application = await _mediator.Send(new CreateLoanApplicationCommand(
            userId,
            request.LoanType,
            request.Amount,
            request.Currency,
            request.PeriodInMonths));

        return CreatedAtAction(nameof(GetLoanApplication), new { id = application.Id }, application);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<LoanApplicationDto>> UpdateLoanApplication(int id,
        [FromBody] UpdateLoanApplicationRequest request)
    {
        var userId = GetUserId();
        
        await _mediator.Send(new UpdateLoanApplicationCommand(
            userId, id, request.LoanType, request.Amount, request.Currency, request.PeriodInMonths));
        return Ok();
    }

    [HttpPost("{id}/submit")]
    public async Task<ActionResult> SubmitLoanApplication(int id)
    {
        var userId = GetUserId();
        await _mediator.Send(new SubmitLoanApplicationCommand(id, userId));
        return Ok();
    }

    [HttpGet("pending")]
    [Authorize(Roles = Roles.Approver)]
    public async Task<ActionResult<IEnumerable<LoanApplicationDto>>> GetPendingLoanApplications()
    {
        var applications = await _mediator.Send(new GetPendingLoanApplicationsQuery());
        return Ok(applications);
    }

    [HttpPost("{id}/approve")]
    [Authorize(Roles = Roles.Approver)]
    public async Task<ActionResult> ApproveLoanApplication(int id)
    {
        var approverId = GetUserId();
        await _mediator.Send(new ApproveLoanApplicationCommand(id, approverId));
        return Ok();
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = Roles.Approver)]
    public async Task<ActionResult> RejectLoanApplication(int id, [FromBody] RejectLoanRequest request)
    {
        var approverId = GetUserId();
        await _mediator.Send(new RejectLoanApplicationCommand(id, approverId, request.RejectionReason));
        return Ok(new { message = "Loan application rejected successfully" });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteLoanApplication(int id)
    {
        var userId = GetUserId();
        await _mediator.Send(new DeleteLoanApplicationCommand(id, userId));
        return NoContent();
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    private string GetUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value ?? "User";
    }
}