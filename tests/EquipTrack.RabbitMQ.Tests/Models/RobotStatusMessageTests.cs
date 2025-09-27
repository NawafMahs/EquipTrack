using EquipTrack.Infrastructure.RabbitMQ.Models;
using FluentAssertions;
using System.Text.Json;
using Xunit;

namespace EquipTrack.RabbitMQ.Tests.Models;

/// <summary>
/// Unit tests for RobotStatusMessage model.
/// </summary>
public sealed class RobotStatusMessageTests
{
    [Fact]
    public void Create_WithValidParameters_ShouldCreateStatusMessage()
    {
        // Arrange
        const string robotId = "robot-001";
        const RobotOperationalStatus status = RobotOperationalStatus.Idle;

        // Act
        var statusMessage = RobotStatusMessage.Create(robotId, status);

        // Assert
        statusMessage.Should().NotBeNull();
        statusMessage.RobotId.Should().Be(robotId);
        statusMessage.Status.Should().Be(status);
        statusMessage.MessageId.Should().NotBeEmpty();
        statusMessage.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        statusMessage.SensorReadings.Should().NotBeNull();
        statusMessage.ErrorCodes.Should().NotBeNull();
        statusMessage.Warnings.Should().NotBeNull();
        statusMessage.Metadata.Should().NotBeNull();
    }

    [Fact]
    public void CreateWithSensorData_ShouldIncludeSensorReadings()
    {
        // Arrange
        const string robotId = "robot-001";
        const RobotOperationalStatus status = RobotOperationalStatus.Producing;
        var sensorReadings = new Dictionary<string, decimal>
        {
            ["temperature"] = 25.5m,
            ["pressure"] = 1013.25m,
            ["humidity"] = 45.0m
        };

        // Act
        var statusMessage = RobotStatusMessage.CreateWithSensorData(robotId, status, sensorReadings);

        // Assert
        statusMessage.Should().NotBeNull();
        statusMessage.RobotId.Should().Be(robotId);
        statusMessage.Status.Should().Be(status);
        statusMessage.SensorReadings.Should().HaveCount(3);
        statusMessage.SensorReadings.Should().ContainKey("temperature");
        statusMessage.SensorReadings.Should().ContainKey("pressure");
        statusMessage.SensorReadings.Should().ContainKey("humidity");
        statusMessage.SensorReadings["temperature"].Should().Be(25.5m);
        statusMessage.SensorReadings["pressure"].Should().Be(1013.25m);
        statusMessage.SensorReadings["humidity"].Should().Be(45.0m);
    }

    [Fact]
    public void CreateErrorStatus_ShouldIncludeErrorCodes()
    {
        // Arrange
        const string robotId = "robot-001";
        var errorCodes = new[] { "ERR001", "ERR002", "WARN003" };

        // Act
        var statusMessage = RobotStatusMessage.CreateErrorStatus(robotId, errorCodes);

        // Assert
        statusMessage.Should().NotBeNull();
        statusMessage.RobotId.Should().Be(robotId);
        statusMessage.Status.Should().Be(RobotOperationalStatus.Error);
        statusMessage.ErrorCodes.Should().HaveCount(3);
        statusMessage.ErrorCodes.Should().Contain("ERR001");
        statusMessage.ErrorCodes.Should().Contain("ERR002");
        statusMessage.ErrorCodes.Should().Contain("WARN003");
    }

    [Fact]
    public void JsonSerialization_ShouldUseKebabCaseNaming()
    {
        // Arrange
        var statusMessage = RobotStatusMessage.Create("robot-001", RobotOperationalStatus.Producing);
        statusMessage.BatteryLevel = 85.5m;
        statusMessage.OperatingHours = 1234.5m;
        statusMessage.FirmwareVersion = "v1.2.3";

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower
        };

        // Act
        var json = JsonSerializer.Serialize(statusMessage, options);
        var deserialized = JsonSerializer.Deserialize<RobotStatusMessage>(json, options);

        // Assert
        json.Should().Contain("\"message-id\":");
        json.Should().Contain("\"robot-id\":");
        json.Should().Contain("\"sensor-readings\":");
        json.Should().Contain("\"error-codes\":");
        json.Should().Contain("\"battery-level\":");
        json.Should().Contain("\"operating-hours\":");
        json.Should().Contain("\"firmware-version\":");
        
        deserialized.Should().NotBeNull();
        deserialized!.MessageId.Should().Be(statusMessage.MessageId);
        deserialized.RobotId.Should().Be(statusMessage.RobotId);
        deserialized.Status.Should().Be(statusMessage.Status);
        deserialized.BatteryLevel.Should().Be(statusMessage.BatteryLevel);
        deserialized.OperatingHours.Should().Be(statusMessage.OperatingHours);
        deserialized.FirmwareVersion.Should().Be(statusMessage.FirmwareVersion);
    }

    [Theory]
    [InlineData(RobotOperationalStatus.Unknown)]
    [InlineData(RobotOperationalStatus.Offline)]
    [InlineData(RobotOperationalStatus.Idle)]
    [InlineData(RobotOperationalStatus.Starting)]
    [InlineData(RobotOperationalStatus.Producing)]
    [InlineData(RobotOperationalStatus.Paused)]
    [InlineData(RobotOperationalStatus.Stopping)]
    [InlineData(RobotOperationalStatus.Error)]
    [InlineData(RobotOperationalStatus.Maintenance)]
    [InlineData(RobotOperationalStatus.EmergencyStop)]
    public void RobotOperationalStatus_ShouldSerializeCorrectly(RobotOperationalStatus status)
    {
        // Arrange
        var statusMessage = RobotStatusMessage.Create("robot-001", status);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower
        };

        // Act
        var json = JsonSerializer.Serialize(statusMessage, options);
        var deserialized = JsonSerializer.Deserialize<RobotStatusMessage>(json, options);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Status.Should().Be(status);
    }

    [Fact]
    public void ProductionInfo_ShouldSerializeCorrectly()
    {
        // Arrange
        var statusMessage = RobotStatusMessage.Create("robot-001", RobotOperationalStatus.Producing);
        statusMessage.ProductionInfo = new ProductionInfo
        {
            RecipeId = "recipe-123",
            BatchNumber = "batch-456",
            StartedAt = DateTime.UtcNow.AddHours(-2),
            EstimatedCompletion = DateTime.UtcNow.AddHours(1),
            ProgressPercentage = 65.5m,
            CurrentStep = "mixing",
            TotalSteps = 10,
            CurrentStepNumber = 7
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower
        };

        // Act
        var json = JsonSerializer.Serialize(statusMessage, options);
        var deserialized = JsonSerializer.Deserialize<RobotStatusMessage>(json, options);

        // Assert
        json.Should().Contain("\"production-info\":");
        json.Should().Contain("\"recipe-id\":");
        json.Should().Contain("\"batch-number\":");
        json.Should().Contain("\"progress-percentage\":");
        json.Should().Contain("\"current-step\":");
        json.Should().Contain("\"total-steps\":");
        json.Should().Contain("\"current-step-number\":");

        deserialized.Should().NotBeNull();
        deserialized!.ProductionInfo.Should().NotBeNull();
        deserialized.ProductionInfo!.RecipeId.Should().Be("recipe-123");
        deserialized.ProductionInfo.BatchNumber.Should().Be("batch-456");
        deserialized.ProductionInfo.ProgressPercentage.Should().Be(65.5m);
        deserialized.ProductionInfo.CurrentStep.Should().Be("mixing");
        deserialized.ProductionInfo.TotalSteps.Should().Be(10);
        deserialized.ProductionInfo.CurrentStepNumber.Should().Be(7);
    }

    [Fact]
    public void NetworkInfo_ShouldSerializeCorrectly()
    {
        // Arrange
        var statusMessage = RobotStatusMessage.Create("robot-001", RobotOperationalStatus.Idle);
        statusMessage.NetworkInfo = new NetworkInfo
        {
            SignalStrength = 85.5m,
            IpAddress = "192.168.1.100",
            ConnectionType = "wifi",
            LastCommunication = DateTime.UtcNow.AddMinutes(-5)
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower
        };

        // Act
        var json = JsonSerializer.Serialize(statusMessage, options);
        var deserialized = JsonSerializer.Deserialize<RobotStatusMessage>(json, options);

        // Assert
        json.Should().Contain("\"network-info\":");
        json.Should().Contain("\"signal-strength\":");
        json.Should().Contain("\"ip-address\":");
        json.Should().Contain("\"connection-type\":");
        json.Should().Contain("\"last-communication\":");

        deserialized.Should().NotBeNull();
        deserialized!.NetworkInfo.Should().NotBeNull();
        deserialized.NetworkInfo!.SignalStrength.Should().Be(85.5m);
        deserialized.NetworkInfo.IpAddress.Should().Be("192.168.1.100");
        deserialized.NetworkInfo.ConnectionType.Should().Be("wifi");
    }

    [Fact]
    public void ComplexStatusMessage_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var statusMessage = RobotStatusMessage.Create("robot-001", RobotOperationalStatus.Producing);
        statusMessage.SensorReadings = new Dictionary<string, decimal>
        {
            ["temperature"] = 25.5m,
            ["pressure"] = 1013.25m
        };
        statusMessage.ErrorCodes = new List<string> { "WARN001" };
        statusMessage.Warnings = new List<string> { "Temperature approaching limit" };
        statusMessage.BatteryLevel = 75.0m;
        statusMessage.OperatingHours = 2500.5m;
        statusMessage.FirmwareVersion = "v2.1.0";
        statusMessage.CorrelationId = "correlation-123";
        statusMessage.Metadata = new Dictionary<string, string>
        {
            ["location"] = "factory-floor-1",
            ["operator"] = "john-doe"
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower
        };

        // Act
        var json = JsonSerializer.Serialize(statusMessage, options);
        var deserialized = JsonSerializer.Deserialize<RobotStatusMessage>(json, options);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.RobotId.Should().Be(statusMessage.RobotId);
        deserialized.Status.Should().Be(statusMessage.Status);
        deserialized.SensorReadings.Should().BeEquivalentTo(statusMessage.SensorReadings);
        deserialized.ErrorCodes.Should().BeEquivalentTo(statusMessage.ErrorCodes);
        deserialized.Warnings.Should().BeEquivalentTo(statusMessage.Warnings);
        deserialized.BatteryLevel.Should().Be(statusMessage.BatteryLevel);
        deserialized.OperatingHours.Should().Be(statusMessage.OperatingHours);
        deserialized.FirmwareVersion.Should().Be(statusMessage.FirmwareVersion);
        deserialized.CorrelationId.Should().Be(statusMessage.CorrelationId);
        deserialized.Metadata.Should().BeEquivalentTo(statusMessage.Metadata);
    }
}