using Microsoft.Extensions.Logging;
using EquipTrack.Domain.Entities;
using EquipTrack.Domain.Enums;
using EquipTrack.Infrastructure.Services;

namespace EquipTrack.Infrastructure.Data;

public class DataSeeder
{
    private readonly EquipTrackDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly ILogger<DataSeeder> _logger;

    public DataSeeder(
        EquipTrackDbContext context,
        IPasswordService passwordService,
        ILogger<DataSeeder> logger)
    {
        _context = context;
        _passwordService = passwordService;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Check if data already exists
            if (_context.Users.Any())
            {
                _logger.LogInformation("Database already contains data. Skipping seed.");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            // Seed Users
            await SeedUsersAsync();

            // Save changes
            await _context.SaveChangesAsync();

            _logger.LogInformation("Database seeding completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedUsersAsync()
    {
        var users = new[]
        {
            new User
            {
                Id = Guid.NewGuid(),
                Email = "admin@equiptrack.com",
                FirstName = "System",
                LastName = "Administrator",
                PasswordHash = _passwordService.HashPassword("Admin123!"),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "manager@equiptrack.com",
                FirstName = "Maintenance",
                LastName = "Manager",
                PasswordHash = _passwordService.HashPassword("Manager123!"),
                Role = UserRole.Manager,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = Guid.NewGuid(),
                Email = "tech@equiptrack.com",
                FirstName = "John",
                LastName = "Technician",
                PasswordHash = _passwordService.HashPassword("Tech123!"),
                Role = UserRole.Technician,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        };

        await _context.Users.AddRangeAsync(users);
        _logger.LogInformation("Seeded {Count} users", users.Length);
    }
}