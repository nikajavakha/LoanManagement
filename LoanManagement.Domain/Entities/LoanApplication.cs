using LoanManagement.Domain.Constants;
using LoanManagement.Domain.Enums;

namespace LoanManagement.Domain.Entities;

public class LoanApplication
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public LoanType LoanType { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public int PeriodInMonths { get; set; }
    public LoanStatus Status { get; set; }
    public int? ApproverId { get; set; }
    public User? Approver { get; set; }
    public string? RejectionReason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public DateTime? PendingAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? RejectedAt  { get; set; }
    
    public bool CanBeEdited() => Status == LoanStatus.InProcess;
    
    public bool CanBeSubmitted() => Status == LoanStatus.InProcess;
    
    public bool CanBeReviewed() => Status == LoanStatus.Submitted;
    
    public bool CanBeApproved() => Status == LoanStatus.Pending;
    
    public bool CanBeRejected() => Status == LoanStatus.Pending;
    
    public void Update(LoanType loanType, decimal amount, Currency currency, int periodInMonths)
    {
        if (!CanBeEdited())
            throw new InvalidOperationException("Cannot edit loan application in current status");
        
        LoanType = loanType;
        Amount = amount;
        Currency = currency;
        PeriodInMonths = periodInMonths;
        UpdatedAt = DateTime.UtcNow;
    }
    
    public void Submit()
    {
        if (!CanBeSubmitted())
            throw new InvalidOperationException("Loan application cannot be submitted in current status");
            
        var now = DateTime.UtcNow;
        Status = LoanStatus.Submitted;
        SubmittedAt = now;
        UpdatedAt = now;
    }
    
    public void MoveToPending()
    {
        if (!CanBeReviewed())
            throw new InvalidOperationException("Loan application cannot be reviewed in current status");
            
        var now = DateTime.UtcNow;
        Status = LoanStatus.Pending;
        PendingAt = now;
        UpdatedAt = now;
    }
    
    public void Approve(int approverId)
    {
        if (!CanBeApproved())
            throw new InvalidOperationException("Loan application cannot be approved in current status");
          
        var now = DateTime.UtcNow;
        Status = LoanStatus.Approved;
        ApproverId = approverId;
        ApprovedAt = now;
        UpdatedAt = now;
        RejectionReason = null;
    }
    
    public void Reject(int approverId, string rejectionReason)
    {
        if (!CanBeRejected())
            throw new InvalidOperationException("Loan application cannot be rejected in current status");
            
        if (string.IsNullOrWhiteSpace(rejectionReason))
            throw new ArgumentException("Rejection reason is required");
            
        var now = DateTime.UtcNow;
        Status = LoanStatus.Rejected;
        ApproverId = approverId;
        RejectionReason = rejectionReason;
        RejectedAt = now;
        UpdatedAt = now;
    }
}