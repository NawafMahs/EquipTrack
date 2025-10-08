using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Net;
using Xunit;
using FluentAssertions;
using EquipTrack.Application.DTOs;
using EquipTrack.Infrastructure.Data;
using EquipTrack.Domain.Enums;

namespace EquipTrack.Tests.Integration;

public class AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add InMemory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_ShouldReturnCreated()
    {
        // Arrange
        var registerDto = new CreateUserCommand
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Role = UserRole.Technician
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<UserQuery>();
        result.Should().NotBeNull();
        result!.Email.Should().Be(registerDto.Email);
        result.FirstName.Should().Be(registerDto.FirstName);
        result.LastName.Should().Be(registerDto.LastName);
    }

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange - First register a user
        var registerDto = new CreateUserCommand
        {
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@test.com",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Role = UserRole.Manager
        };

        await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);

        var loginDto = new LoginCommand
        {
            Email = registerDto.Email,
            Password = registerDto.Password
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<LoginQuery>();
        result.Should().NotBeNull();
        result!.Token.Should().NotBeNullOrEmpty();
        result.User.Email.Should().Be(loginDto.Email);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var loginDto = new LoginCommand
        {
            Email = "nonexistent@test.com",
            Password = "WrongPassword"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/login", loginDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ShouldReturnConflict()
    {
        // Arrange
        var registerDto = new CreateUserCommand
        {
            FirstName = "Test",
            LastName = "User",
            Email = "duplicate@test.com",
            Password = "Password123!",
            PhoneNumber = "1234567890",
            Role = UserRole.Technician
        };

        // Register first user
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerDto);

        // Try to register with same email
        var duplicateDto = new CreateUserCommand
        {
            FirstName = "Another",
            LastName = "User",
            Email = "duplicate@test.com",
            Password = "Password456!",
            PhoneNumber = "0987654321",
            Role = UserRole.Admin
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/auth/register", duplicateDto);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}