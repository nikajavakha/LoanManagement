using LoanManagement.Domain.Constants;
using LoanManagement.Domain.Entities;

namespace LoanManagement.Domain;

public class User
{
    public int Id { get; set; }
    public string PersonalNumber { get; set; }
    public string PasswordHash { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Role { get; set; } = Roles.User;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public ICollection<LoanApplication> LoanApplications { get; set; } = new List<LoanApplication>();
    
    public string GetFullName() => $"{FirstName} {LastName}";
}