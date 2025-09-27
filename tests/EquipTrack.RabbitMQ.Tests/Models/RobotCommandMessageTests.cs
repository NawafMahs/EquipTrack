using EquipTrack.Infrastructure.RabbitMQ.Models;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace EquipTrack.RabbitMQ.Tests.Models;

/// <summary>
/// Unit tests for RobotCommandMessage model.
/// </summary>
public sealed class RobotCommandMessageTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateCommandMessage()
    {
        // Arrange
        const string robotId = "robot-001";
        const string commandType = "start-production";
        const string initiatedBy = "operator-1";
        var parameters = new Dictionary<string, object> { ["recipe-id"] = "recipe-123" };

        // Act
        var command = RobotCommandMessage.Create(robotId, commandType, initiatedBy, parameters);

        // Assert
        command.Should().NotBeNull();
        command.RobotId.Should().Be(robotId);
        command.CommandType.Should().Be(commandType);
        command.InitiatedBy.Should().Be(initiatedBy);
        command.Parameters.Should().ContainKey("recipe-id");
        command.Parameters["recipe-id"].Should().Be("recipe-123");
        command.MessageId.Should().NotBeEmpty();
        command.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        command.ExpiresAt.Should().BeAfter(DateTime.UtcNow);
        command.Priority.Should().Be(CommandPriority.Normal);
        command.RequiresAcknowledgment.Should().BeTrue();
    }

    [Fact]
    public void CreateStartProductionCommand_ShouldCreateCorrectCommand()
    {
        // Arrange
        const string robotId = "robot-001";
        const string recipeId = "recipe-123";
        const string batchNumber = "batch-456";
        const string initiatedBy = "operator-1";

        // Act
        var command = RobotCommandMessage.CreateStartProductionCommand(robotId, recipeId, batchNumber, initiatedBy);

        // Assert
        command.Should().NotBeNull();
        command.RobotId.Should().Be(robotId);
        command.CommandType.Should().Be("start-production");
        command.InitiatedBy.Should().Be(initiatedBy);
        command.Priority.Should().Be(CommandPriority.High);
        command.Parameters.Should().ContainKey("recipe-id");
        command.Parameters.Should().ContainKey("batch-number");
        command.Parameters["recipe-id"].Should().Be(recipeId);
        command.Parameters["batch-number"].Should().Be(batchNumber);
    }

    [Fact]
    public void CreateStopProductionCommand_ShouldCreateCorrectCommand()
    {
        // Arrange
        const string robotId = "robot-001";
        const string reason = "maintenance-required";
        const string initiatedBy = "operator-1";

        // Act
        var command = RobotCommandMessage.CreateStopProductionCommand(robotId, reason, initiatedBy);

        // Assert
        command.Should().NotBeNull();
        command.RobotId.Should().Be(robotId);
        command.CommandType.Should().Be("stop-production");
        command.InitiatedBy.Should().Be(initiatedBy);
        command.Priority.Should().Be(CommandPriority.High);
        command.Parameters.Should().ContainKey("reason");
        command.Parameters["reason"].Should().Be(reason);
    }

    [Fact]
    public void CreateEmergencyStopCommand_ShouldCreateCorrectCommand()
    {
        // Arrange
        const string robotId = "robot-001";
        const string initiatedBy = "operator-1";

        // Act
        var command = RobotCommandMessage.CreateEmergencyStopCommand(robotId, initiatedBy);

        // Assert
        command.Should().NotBeNull();
        command.RobotId.Should().Be(robotId);
        command.CommandType.Should().Be("emergency-stop");
        command.InitiatedBy.Should().Be(initiatedBy);
        command.Priority.Should().Be(CommandPriority.Critical);
    }

    [Fact]
    public void CreateStatusRequestCommand_ShouldCreateCorrectCommand()
    {
        // Arrange
        const string robotId = "robot-001";
        const string initiatedBy = "system";

        // Act
        var command = RobotCommandMessage.CreateStatusRequestCommand(robotId, initiatedBy);

        // Assert
        command.Should().NotBeNull();
        command.RobotId.Should().Be(robotId);
        command.CommandType.Should().Be("request-status");
        command.InitiatedBy.Should().Be(initiatedBy);
        command.Priority.Should().Be(CommandPriority.Low);
    }

    [Fact]
    public void JsonSerialization_ShouldUseKebabCaseNaming()
    {
        // Arrange
        var command = RobotCommandMessage.Create("robot-001", "test-command", "operator-1");
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower
        };

        // Act
        var json = JsonSerializer.Serialize(command, options);
        var deserialized = JsonSerializer.Deserialize<RobotCommandMessage>(json, options);

        // Assert
        json.Should().Contain("\"message-id\":");
        json.Should().Contain("\"robot-id\":");
        json.Should().Contain("\"command-type\":");
        json.Should().Contain("\"created-at\":");
        json.Should().Contain("\"initiated-by\":");
        
        deserialized.Should().NotBeNull();
        deserialized!.MessageId.Should().Be(command.MessageId);
        deserialized.RobotId.Should().Be(command.RobotId);
        deserialized.CommandType.Should().Be(command.CommandType);
        deserialized.InitiatedBy.Should().Be(command.InitiatedBy);
    }

    [Theory]
    [InlineData(CommandPriority.Low)]
    [InlineData(CommandPriority.Normal)]
    [InlineData(CommandPriority.High)]
    [InlineData(CommandPriority.Critical)]
    public void CommandPriority_ShouldSerializeCorrectly(CommandPriority priority)
    {
        // Arrange
        var command = RobotCommandMessage.Create("robot-001", "test-command", "operator-1", priority: priority);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower
        };

        // Act
        var json = JsonSerializer.Serialize(command, options);
        var deserialized = JsonSerializer.Deserialize<RobotCommandMessage>(json, options);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Priority.Should().Be(priority);
    }

    [Fact]
    public void Metadata_ShouldBeSerializable()
    {
        // Arrange
        var command = RobotCommandMessage.Create("robot-001", "test-command", "operator-1");
        command.Metadata["custom-key"] = "custom-value";
        command.Metadata["another-key"] = "another-value";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower
        };

        // Act
        var json = JsonSerializer.Serialize(command, options);
        var deserialized = JsonSerializer.Deserialize<RobotCommandMessage>(json, options);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Metadata.Should().ContainKey("custom-key");
        deserialized.Metadata.Should().ContainKey("another-key");
        deserialized.Metadata["custom-key"].Should().Be("custom-value");
        deserialized.Metadata["another-key"].Should().Be("another-value");
    }
}