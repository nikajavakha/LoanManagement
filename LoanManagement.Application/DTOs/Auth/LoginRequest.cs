namespace LoanManagement.Application.DTOs.Auth;

public class LoginRequest
{
    public string PersonalNumber { get; set; }
    public string Password { get; set; }
}