using LoanManagement.Application.Interfaces;
using LoanManagement.Application.Interfaces.Services;
using LoanManagement.Domain;
using LoanManagement.Domain.Constants;
using LoanManagement.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LoanManagement.Infrastructure.Extensions;

public static class DatabaseSeeder
{
    public static async Task SeedDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;

        var context = services.GetRequiredService<ApplicationDbContext>();
        var passwordHasher = services.GetRequiredService<IPasswordHasher>();
        var logger = services.GetRequiredService<ILogger<IApplicationDbContext>>();

        try
        {
            await context.Database.MigrateAsync();

            var approverExists = await context.Users.AnyAsync(u => u.Role == Roles.Approver);

            if (!approverExists)
            {
                var approverUser = new User
                {
                    PersonalNumber = "00000000001",
                    PasswordHash = passwordHasher.HashPassword("Approver123"),
                    FirstName = "System",
                    LastName = "Approver",
                    PhoneNumber = "+995599000001",
                    DateOfBirth = new DateTime(1993, 1, 1),
                    Role = Roles.Approver,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                await context.Users.AddAsync(approverUser);
                await context.SaveChangesAsync();

                logger?.LogInformation("Approver user seeded successfully");
            }
            else
            {
                logger?.LogInformation("Approver user already exists, skipping seed");
            }
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, "Error occurred while seeding approver user");
        }
    }
}