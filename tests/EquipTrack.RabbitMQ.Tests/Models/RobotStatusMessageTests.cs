using EquipTrack.Domain.Enums;
using EquipTrack.RabbitMQ.Models;
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
        const string robotId = "550e8400-e29b-41d4-a716-446655440000";
        const RobotStatus status = RobotStatus.Idle;

        // Act
        var statusMessage = RobotStatusMessage.Create(robotId, status);

        // Assert
        statusMessage.Should().NotBeNull();
        statusMessage.RobotId.Should().NotBeEmpty();
        statusMessage.Status.Should().Be(status);
        statusMessage.MessageId.Should().NotBeEmpty();
        statusMessage.Timestamp.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void CreateWithSensorData_ShouldIncludeSensorReadings()
    {
        // Arrange
        const string robotId = "550e8400-e29b-41d4-a716-446655440000";
        const RobotStatus status = RobotStatus.Running;
        var sensorReadings = new List<SensorReading>
        {
            new() { SensorId = "temp-001", SensorType = "temperature", Value = 25.5m, Unit = "C", ReadingTime = DateTime.UtcNow },
            new() { SensorId = "press-001", SensorType = "pressure", Value = 1013.25m, Unit = "hPa", ReadingTime = DateTime.UtcNow },
            new() { SensorId = "hum-001", SensorType = "humidity", Value = 45.0m, Unit = "%", ReadingTime = DateTime.UtcNow }
        };

        // Act
        var statusMessage = RobotStatusMessage.Create(robotId, status) with { SensorReadings = sensorReadings };

        // Assert
        statusMessage.Should().NotBeNull();
        statusMessage.RobotId.Should().NotBeEmpty();
        statusMessage.Status.Should().Be(status);
        statusMessage.SensorReadings.Should().HaveCount(3);
        statusMessage.SensorReadings.Should().Contain(s => s.SensorType == "temperature" && s.Value == 25.5m);
        statusMessage.SensorReadings.Should().Contain(s => s.SensorType == "pressure" && s.Value == 1013.25m);
        statusMessage.SensorReadings.Should().Contain(s => s.SensorType == "humidity" && s.Value == 45.0m);
    }

    [Fact]
    public void CreateErrorStatus_ShouldIncludeErrorCodes()
    {
        // Arrange
        const string robotId = "550e8400-e29b-41d4-a716-446655440000";
        var errorCodes = new List<string> { "ERR001", "ERR002", "WARN003" };

        // Act
        var statusMessage = RobotStatusMessage.Create(robotId, RobotStatus.Error) with { ErrorCodes = errorCodes };

        // Assert
        statusMessage.Should().NotBeNull();
        statusMessage.RobotId.Should().NotBeEmpty();
        statusMessage.Status.Should().Be(RobotStatus.Error);
        statusMessage.ErrorCodes.Should().HaveCount(3);
        statusMessage.ErrorCodes.Should().Contain("ERR001");
        statusMessage.ErrorCodes.Should().Contain("ERR002");
        statusMessage.ErrorCodes.Should().Contain("WARN003");
    }

    [Fact]
    public void JsonSerialization_ShouldSerializeCorrectly()
    {
        // Arrange
        var statusMessage = RobotStatusMessage.Create("550e8400-e29b-41d4-a716-446655440000", RobotStatus.Running) with
        {
            BatteryLevel = 85.5m,
            OperatingHours = 1234.5m,
            FirmwareVersion = "v1.2.3"
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Act
        var json = JsonSerializer.Serialize(statusMessage, options);
        var deserialized = JsonSerializer.Deserialize<RobotStatusMessage>(json, options);

        // Assert
        json.Should().Contain("\"messageId\":");
        json.Should().Contain("\"robotId\":");
        json.Should().Contain("\"batteryLevel\":");
        json.Should().Contain("\"operatingHours\":");
        json.Should().Contain("\"firmwareVersion\":");
        
        deserialized.Should().NotBeNull();
        deserialized!.MessageId.Should().Be(statusMessage.MessageId);
        deserialized.RobotId.Should().Be(statusMessage.RobotId);
        deserialized.Status.Should().Be(statusMessage.Status);
        deserialized.BatteryLevel.Should().Be(statusMessage.BatteryLevel);
        deserialized.OperatingHours.Should().Be(statusMessage.OperatingHours);
        deserialized.FirmwareVersion.Should().Be(statusMessage.FirmwareVersion);
    }

    [Theory]
    [InlineData(RobotStatus.Idle)]
    [InlineData(RobotStatus.Running)]
    [InlineData(RobotStatus.Charging)]
    [InlineData(RobotStatus.Error)]
    [InlineData(RobotStatus.Maintenance)]
    [InlineData(RobotStatus.OutOfService)]
    public void RobotStatus_ShouldSerializeCorrectly(RobotStatus status)
    {
        // Arrange
        var statusMessage = RobotStatusMessage.Create("550e8400-e29b-41d4-a716-446655440000", status);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
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
        var statusMessage = RobotStatusMessage.Create("550e8400-e29b-41d4-a716-446655440000", RobotStatus.Running) with
        {
            ProductionInfo = new ProductionInfo
            {
                RecipeId = "recipe-123",
                BatchNumber = "batch-456",
                StartedAt = DateTime.UtcNow.AddHours(-2),
                EstimatedCompletion = DateTime.UtcNow.AddHours(1),
                ProgressPercentage = 65.5m,
                CurrentStep = "mixing",
                TotalSteps = 10,
                CurrentStepNumber = 7
            }
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Act
        var json = JsonSerializer.Serialize(statusMessage, options);
        var deserialized = JsonSerializer.Deserialize<RobotStatusMessage>(json, options);

        // Assert
        json.Should().Contain("\"productionInfo\":");
        json.Should().Contain("\"recipeId\":");
        json.Should().Contain("\"batchNumber\":");
        json.Should().Contain("\"progressPercentage\":");
        json.Should().Contain("\"currentStep\":");
        json.Should().Contain("\"totalSteps\":");
        json.Should().Contain("\"currentStepNumber\":");

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
        var statusMessage = RobotStatusMessage.Create("550e8400-e29b-41d4-a716-446655440000", RobotStatus.Idle) with
        {
            NetworkInfo = new NetworkInfo
            {
                SignalStrength = 85,
                IpAddress = "192.168.1.100",
                ConnectionType = "wifi",
                LastCommunication = DateTime.UtcNow.AddMinutes(-5),
                IsConnected = true
            }
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Act
        var json = JsonSerializer.Serialize(statusMessage, options);
        var deserialized = JsonSerializer.Deserialize<RobotStatusMessage>(json, options);

        // Assert
        json.Should().Contain("\"networkInfo\":");
        json.Should().Contain("\"signalStrength\":");
        json.Should().Contain("\"ipAddress\":");
        json.Should().Contain("\"connectionType\":");
        json.Should().Contain("\"lastCommunication\":");

        deserialized.Should().NotBeNull();
        deserialized!.NetworkInfo.Should().NotBeNull();
        deserialized.NetworkInfo!.SignalStrength.Should().Be(85);
        deserialized.NetworkInfo.IpAddress.Should().Be("192.168.1.100");
        deserialized.NetworkInfo.ConnectionType.Should().Be("wifi");
    }

    [Fact]
    public void ComplexStatusMessage_ShouldSerializeAndDeserializeCorrectly()
    {
        // Arrange
        var statusMessage = RobotStatusMessage.Create("550e8400-e29b-41d4-a716-446655440000", RobotStatus.Running) with
        {
            SensorReadings = new List<SensorReading>
            {
                new() { SensorId = "temp-001", SensorType = "temperature", Value = 25.5m, Unit = "C", ReadingTime = DateTime.UtcNow },
                new() { SensorId = "press-001", SensorType = "pressure", Value = 1013.25m, Unit = "hPa", ReadingTime = DateTime.UtcNow }
            },
            ErrorCodes = new List<string> { "WARN001" },
            Warnings = new List<string> { "Temperature approaching limit" },
            BatteryLevel = 75.0m,
            OperatingHours = 2500.5m,
            FirmwareVersion = "v2.1.0",
            CorrelationId = "correlation-123"
        };

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Act
        var json = JsonSerializer.Serialize(statusMessage, options);
        var deserialized = JsonSerializer.Deserialize<RobotStatusMessage>(json, options);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.RobotId.Should().Be(statusMessage.RobotId);
        deserialized.Status.Should().Be(statusMessage.Status);
        deserialized.SensorReadings.Should().HaveCount(2);
        deserialized.ErrorCodes.Should().BeEquivalentTo(statusMessage.ErrorCodes);
        deserialized.Warnings.Should().BeEquivalentTo(statusMessage.Warnings);
        deserialized.BatteryLevel.Should().Be(statusMessage.BatteryLevel);
        deserialized.OperatingHours.Should().Be(statusMessage.OperatingHours);
        deserialized.FirmwareVersion.Should().Be(statusMessage.FirmwareVersion);
        deserialized.CorrelationId.Should().Be(statusMessage.CorrelationId);
    }
}