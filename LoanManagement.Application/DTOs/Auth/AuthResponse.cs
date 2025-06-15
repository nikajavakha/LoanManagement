namespace LoanManagement.Application.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; }
    public string Role { get; set; }
    public DateTime ExpiresAt { get; set; }
}