using LoanManagement.Domain.Enums;

namespace LoanManagement.Application.DTOs.Loan;

public class LoanApplicationDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string UserFullName { get; set; }
    public LoanType LoanType { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public int PeriodInMonths { get; set; }
    public LoanStatus Status { get; set; }
    public int? ApproverId { get; set; }
    public string? ApproverFullName { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? ReviewStartedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    
    public bool CanEdit { get; set; }
    public bool CanSubmit { get; set; }
    public bool CanApprove { get; set; }
    public bool CanReject { get; set; }
    public bool CanDelete { get; set; }
}