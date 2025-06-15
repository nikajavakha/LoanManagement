using LoanManagement.Domain.Enums;

namespace LoanManagement.Application.DTOs.Loan;

public class CreateLoanApplicationRequest
{
    public LoanType LoanType { get; set; }
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
    public int PeriodInMonths { get; set; }
}