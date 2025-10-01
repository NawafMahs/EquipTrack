# CMMS + IoT Asset Management System - Implementation Guide

## Overview
This document outlines the complete implementation of a production-ready CMMS (Computerized Maintenance Management System) with IoT capabilities for managing machines, robots, sensors, and spare parts.

## Architecture

### Layered Architecture (CQRS Pattern)
```
┌─────────────────────────────────────────┐
│         Presentation Layer              │
│    (EquipTrack.Dashboard.API)          │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│         Application Layer               │
│    (EquipTrack.Application)            │
│    - Commands (Write Operations)        │
│    - Queries (Read Operations)          │
│    - Validators                         │
│    - Handlers                           │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│          Domain Layer                   │
│    (EquipTrack.Domain)                 │
│    - Entities                           │
│    - Value Objects                      │
│    - Domain Events                      │
│    - Repository Interfaces              │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│       Infrastructure Layer              │
│    (EquipTrack.Infrastructure)         │
│    - Repository Implementations         │
│    - EF Core Configurations            │
│    - Data Seeding                       │
└─────────────────────────────────────────┘
                    ↓
┌─────────────────────────────────────────┐
│         Messaging Layer                 │
│    (EquipTrack.RabbitMQ)               │
│    - Event Publishers                   │
│    - Event Consumers                    │
│    - Message Models                     │
└─────────────────────────────────────────┘
```

## Domain Entities

### 1. Machine Entity
**Purpose**: Represents physical machines in the facility.

**Properties**:
- `Id` (Guid): Primary key
- `Name` (string): Machine name
- `SerialNumber` (string): Unique serial number
- `Model` (string): Machine model
- `Manufacturer` (string): Manufacturer name
- `Location` (string): Physical location
- `Status` (MachineStatus): Current operational status
- `InstallationDate` (DateTime): Installation date
- `MachineTypeRef` (string): Reference to machine type
- `OperatingHours` (int): Total operating hours
- `CurrentEfficiency` (decimal?): Current efficiency percentage
- `LastMaintenanceDate` (DateTime?): Last maintenance date
- `NextMaintenanceDate` (DateTime?): Next scheduled maintenance

**Relationships**:
- One-to-Many: MachineSensor
- One-to-Many: MachineLog
- One-to-Many: MachineSparePartUsage
- One-to-Many: WorkOrder

**Key Behaviors**:
- `ChangeStatus()`: Updates machine status with validation
- `RecordMaintenance()`: Records maintenance activities
- `IncrementOperatingHours()`: Tracks usage
- `UpdateEfficiency()`: Updates efficiency metrics
- `AddSensor()`: Attaches sensors
- `AddLog()`: Records operational logs

### 2. Robot Entity
**Purpose**: Represents autonomous robots with advanced capabilities.

**Properties**:
- `Id` (Guid): Primary key
- `Name` (string): Robot name
- `SerialNumber` (string): Unique serial number
- `Model` (string): Robot model
- `Manufacturer` (string): Manufacturer name
- `Location` (string): Current location
- `Status` (RobotStatus): Current operational status
- `RobotType` (RobotType): Type of robot
- `MaxPayloadKg` (int): Maximum payload capacity
- `ReachMeters` (decimal): Maximum reach
- `BatteryLevel` (decimal?): Current battery level
- `CurrentTask` (string?): Active task description
- `CycleCount` (int): Total cycles completed
- `OperatingHours` (int): Total operating hours

**Relationships**:
- One-to-Many: RobotSensor
- One-to-Many: RobotLog
- One-to-Many: RobotSparePartUsage
- One-to-Many: WorkOrder

**Key Behaviors**:
- `ChangeStatus()`: Updates robot status
- `AssignTask()`: Assigns new task
- `CompleteTask()`: Marks task complete
- `UpdateBatteryLevel()`: Updates battery with alerts
- `RequiresCharging()`: Checks if charging needed
- `RecordMaintenance()`: Records maintenance

### 3. MachineSensor & RobotSensor Entities
**Purpose**: Monitor machine/robot conditions in real-time.

**Properties**:
- `Id` (Guid): Primary key
- `MachineRef/RobotRef` (Guid): Parent reference
- `SensorName` (string): Sensor identifier
- `SensorType` (SensorType): Type of sensor
- `Unit` (string): Measurement unit
- `MinThreshold` (decimal?): Minimum acceptable value
- `MaxThreshold` (decimal?): Maximum acceptable value
- `CurrentValue` (decimal?): Latest reading
- `LastReadingTime` (DateTime?): Last reading timestamp
- `ReadingIntervalSeconds` (int): Reading frequency

**Relationships**:
- One-to-Many: SensorReading

**Key Behaviors**:
- `RecordReading()`: Stores new sensor reading
- `SetThresholds()`: Configures alert thresholds
- `IsOutOfThreshold()`: Checks if current value is abnormal
- **Domain Events**: Raises `SensorThresholdBreachedEvent` when thresholds are exceeded

### 4. SensorReading Entity
**Purpose**: Immutable historical record of sensor data.

**Properties**:
- `Id` (Guid): Primary key
- `SensorRef` (Guid): Parent sensor reference
- `Value` (decimal): Reading value
- `Timestamp` (DateTime): Reading time
- `Metadata` (string?): Additional context

### 5. MachineLog & RobotLog Entities
**Purpose**: Audit trail for all machine/robot operations.

**Properties**:
- `Id` (Guid): Primary key
- `MachineRef/RobotRef` (Guid): Parent reference
- `LogType` (MachineLogType/RobotLogType): Type of log entry
- `Message` (string): Log message
- `Severity` (MachineLogSeverity/RobotLogSeverity): Severity level
- `Timestamp` (DateTime): Log timestamp
- `AdditionalData` (string?): Extra information

**Log Types**:
- StatusChange
- Error
- Warning
- Maintenance
- OperationStart/End
- ConfigurationChange
- SensorAlert
- SparePartReplacement
- Calibration
- (Robot-specific) BatteryAlert, CollisionDetected

### 6. MachineSparePartUsage & RobotSparePartUsage Entities
**Purpose**: Track spare part consumption and installation.

**Properties**:
- `Id` (Guid): Primary key
- `MachineRef/RobotRef` (Guid): Parent reference
- `SparePartRef` (Guid): Spare part reference
- `QuantityUsed` (int): Quantity consumed
- `UsageDate` (DateTime): Installation date
- `Reason` (string?): Reason for replacement
- `WorkOrderRef` (Guid?): Associated work order
- `InstalledBy` (string?): Technician name

## Enumerations

### MachineStatus
```csharp
public enum MachineStatus
{
    Idle = 0,
    Running = 1,
    Error = 2,
    Maintenance = 3,
    OutOfService = 4
}
```

### RobotStatus
```csharp
public enum RobotStatus
{
    Idle = 0,
    Running = 1,
    Charging = 2,
    Error = 3,
    Maintenance = 4,
    OutOfService = 5
}
```

### RobotType
```csharp
public enum RobotType
{
    Industrial = 0,
    Collaborative = 1,
    Mobile = 2,
    Articulated = 3,
    SCARA = 4,
    Delta = 5,
    Cartesian = 6,
    AGV = 7
}
```

### SensorType
```csharp
public enum SensorType
{
    Temperature = 0,
    Pressure = 1,
    Vibration = 2,
    Humidity = 3,
    Speed = 4,
    Current = 5,
    Voltage = 6,
    Position = 7,
    Proximity = 8,
    Flow = 9,
    Level = 10,
    Force = 11,
    Torque = 12,
    Gas = 13,
    Smoke = 14
}
```

## CQRS Implementation

### Commands (Write Operations)

#### Machine Commands
1. **CreateMachineCommand**
   - Creates new machine
   - Validator: Ensures all required fields
   - Handler: Persists to database

2. **UpdateMachineStatusCommand**
   - Changes machine status
   - Validator: Validates status transition
   - Handler: Updates status and logs change

3. **RecordMachineMaintenanceCommand**
   - Records maintenance activity
   - Updates maintenance dates
   - Creates maintenance log

4. **AddMachineSensorCommand**
   - Attaches sensor to machine
   - Configures thresholds

5. **RecordSensorReadingCommand**
   - Stores sensor reading
   - Checks thresholds
   - Raises alerts if needed

#### Robot Commands
1. **CreateRobotCommand**
2. **UpdateRobotStatusCommand**
3. **AssignRobotTaskCommand**
4. **CompleteRobotTaskCommand**
5. **UpdateRobotBatteryCommand**
6. **RecordRobotMaintenanceCommand**

### Queries (Read Operations)

#### Machine Queries
1. **GetMachineByIdQuery**
   - Retrieves single machine
   - Includes related data

2. **GetMachinesByStatusQuery**
   - Filters by status
   - Supports pagination

3. **GetMachinesRequiringMaintenanceQuery**
   - Returns machines needing maintenance
   - Based on scheduled dates

4. **GetMachineWithSensorsQuery**
   - Includes all sensors
   - Shows current readings

5. **GetMachineLogsQuery**
   - Retrieves operational logs
   - Filters by date range and severity

#### Robot Queries
1. **GetRobotByIdQuery**
2. **GetRobotsByStatusQuery**
3. **GetRobotsRequiringChargingQuery**
4. **GetRobotWithSensorsQuery**
5. **GetRobotLogsQuery**

#### Sensor Queries
1. **GetSensorReadingsQuery**
   - Historical sensor data
   - Time range filtering

2. **GetSensorReadingsByMachineQuery**
   - All sensors for a machine
   - Latest readings

## Repository Pattern

### Read-Only Repositories
```csharp
public interface IMachineReadOnlyRepository : IReadOnlyRepository<Machine>
{
    Task<IEnumerable<Machine>> GetByStatusAsync(MachineStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Machine>> GetByLocationAsync(string location, CancellationToken cancellationToken = default);
    Task<IEnumerable<Machine>> GetRequiringMaintenanceAsync(CancellationToken cancellationToken = default);
    Task<Machine?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task<Machine?> GetWithSensorsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Machine?> GetWithLogsAsync(Guid id, CancellationToken cancellationToken = default);
}
```

### Write-Only Repositories
```csharp
public interface IMachineWriteOnlyRepository : IWriteOnlyRepository<Machine>
{
    // Inherits: AddAsync, UpdateAsync, DeleteAsync
}
```

## Validation Pipeline

All commands pass through FluentValidation pipeline:

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any()) return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Any())
            throw new ValidationException(failures);

        return await next();
    }
}
```

## RabbitMQ Integration

### Message Models

1. **MachineStatusMessage**
   - Published when machine status changes
   - Consumed by monitoring services

2. **RobotStatusMessage**
   - Published when robot status changes
   - Includes battery level and task info

3. **SensorReadingMessage**
   - Published for each sensor reading
   - Real-time monitoring

4. **MaintenanceAlertMessage**
   - Published when maintenance is due
   - Triggers notifications

### Exchange Configuration
```
Exchange: equiptrack.events
Type: Topic
Routing Keys:
  - machine.status.changed
  - robot.status.changed
  - sensor.reading.recorded
  - sensor.threshold.breached
  - maintenance.due
  - spare.part.low.stock
```

### Publishers
```csharp
public interface IEventPublisher
{
    Task PublishAsync<T>(T message, string routingKey, CancellationToken cancellationToken = default);
}
```

### Consumers
```csharp
public class MachineStatusConsumer : IMessageConsumer<MachineStatusMessage>
{
    public async Task ConsumeAsync(MachineStatusMessage message, CancellationToken cancellationToken)
    {
        // Handle machine status update
        // Update dashboards, send notifications, etc.
    }
}
```

## Database Configuration

### Entity Framework Core Configurations

#### Machine Configuration
```csharp
public class MachineConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> builder)
    {
        builder.ToTable("Machines");
        builder.HasKey(m => m.Id);
        
        builder.Property(m => m.Name).IsRequired().HasMaxLength(200);
        builder.Property(m => m.SerialNumber).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Model).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Manufacturer).IsRequired().HasMaxLength(100);
        builder.Property(m => m.Location).IsRequired().HasMaxLength(200);
        
        builder.HasIndex(m => m.SerialNumber).IsUnique();
        builder.HasIndex(m => m.Status);
        builder.HasIndex(m => m.Location);
        builder.HasIndex(m => m.NextMaintenanceDate);
        
        builder.HasMany(m => m.Sensors)
            .WithOne()
            .HasForeignKey("MachineRef")
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(m => m.Logs)
            .WithOne()
            .HasForeignKey("MachineRef")
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

### Indexes for Performance
- SerialNumber (Unique)
- Status
- Location
- NextMaintenanceDate
- SensorRef + Timestamp (for readings)
- MachineRef/RobotRef (for logs and usage)

## Data Seeding

### Sample Machine Data
```csharp
var machines = new[]
{
    Machine.Create("CNC Mill 001", "CNC-2024-001", "VMC-850", "Haas Automation", "Production Floor A", DateTime.UtcNow.AddYears(-2), "CNC-MILL"),
    Machine.Create("Lathe 001", "LTH-2024-001", "ST-20", "Mazak", "Production Floor A", DateTime.UtcNow.AddYears(-1), "LATHE"),
    Machine.Create("Press 001", "PRS-2024-001", "HP-500", "Schuler", "Production Floor B", DateTime.UtcNow.AddMonths(-6), "PRESS")
};
```

### Sample Robot Data
```csharp
var robots = new[]
{
    Robot.Create("Welding Robot 001", "WR-2024-001", "IRB 6700", "ABB", "Welding Station 1", DateTime.UtcNow.AddYears(-1), RobotType.Articulated, 150, 3.2m),
    Robot.Create("AGV 001", "AGV-2024-001", "MiR200", "Mobile Industrial Robots", "Warehouse", DateTime.UtcNow.AddMonths(-8), RobotType.AGV, 200, 0m),
    Robot.Create("Cobot 001", "CB-2024-001", "UR10e", "Universal Robots", "Assembly Line 1", DateTime.UtcNow.AddMonths(-4), RobotType.Collaborative, 10, 1.3m)
};
```

### Sample Sensor Data
```csharp
var sensors = new[]
{
    MachineSensor.Create(machine1.Id, "Temperature Sensor", SensorType.Temperature, "°C", 30),
    MachineSensor.Create(machine1.Id, "Vibration Sensor", SensorType.Vibration, "mm/s", 60),
    MachineSensor.Create(machine1.Id, "Spindle Current", SensorType.Current, "A", 10)
};

sensors[0].SetThresholds(0, 80); // Temperature: 0-80°C
sensors[1].SetThresholds(0, 10); // Vibration: 0-10 mm/s
sensors[2].SetThresholds(0, 50); // Current: 0-50 A
```

## API Controllers

### MachinesController
```csharp
[ApiController]
[Route("api/[controller]")]
public class MachinesController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateMachineCommand command)
    {
        var result = await Mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await Mediator.Send(new GetMachineByIdQuery(id));
        return result.IsSuccess ? Ok(result) : NotFound(result);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateMachineStatusCommand command)
    {
        var result = await Mediator.Send(command);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("requiring-maintenance")]
    public async Task<IActionResult> GetRequiringMaintenance()
    {
        var result = await Mediator.Send(new GetMachinesRequiringMaintenanceQuery());
        return Ok(result);
    }
}
```

### RobotsController
Similar structure with robot-specific endpoints including:
- Battery status
- Task assignment
- Charging status

### SensorsController
```csharp
[HttpPost("readings")]
public async Task<IActionResult> RecordReading([FromBody] RecordSensorReadingCommand command)
{
    var result = await Mediator.Send(command);
    return result.IsSuccess ? Ok(result) : BadRequest(result);
}

[HttpGet("{sensorId}/readings")]
public async Task<IActionResult> GetReadings(Guid sensorId, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime)
{
    var result = await Mediator.Send(new GetSensorReadingsQuery(sensorId, startTime, endTime));
    return Ok(result);
}
```

## Testing Strategy

### Unit Tests
- Entity behavior tests
- Validator tests
- Handler tests (with mocked repositories)

### Integration Tests
- API endpoint tests
- Database integration tests
- RabbitMQ message flow tests

### Example Test
```csharp
public class CreateMachineCommandHandlerTests
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesMachine()
    {
        // Arrange
        var mockRepo = new Mock<IMachineWriteOnlyRepository>();
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var handler = new CreateMachineCommandHandler(mockRepo.Object, mockUnitOfWork.Object);
        var command = new CreateMachineCommand("Test Machine", "SN-001", "Model-X", "Manufacturer", "Location A", DateTime.UtcNow, "TYPE-001");

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        mockRepo.Verify(r => r.AddAsync(It.IsAny<Machine>(), It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

## Deployment Considerations

### Database Migrations
```bash
dotnet ef migrations add AddMachinesAndRobots --project src/Infrastructure/EquipTrack.Infrastructure
dotnet ef database update --project src/Infrastructure/EquipTrack.Infrastructure
```

### RabbitMQ Setup
```bash
docker run -d --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
```

### Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=EquipTrackDb;Trusted_Connection=True;"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "Exchange": "equiptrack.events"
  }
}
```

## Performance Optimization

### Caching Strategy
- Cache frequently accessed machines/robots
- Cache sensor readings for dashboards
- Invalidate cache on updates

### Query Optimization
- Use projections for list views
- Implement pagination
- Add appropriate indexes
- Use AsNoTracking() for read-only queries

### Background Jobs
- Periodic maintenance checks
- Sensor data aggregation
- Alert generation
- Report generation

## Monitoring & Observability

### Metrics to Track
- Machine uptime/downtime
- Robot battery levels
- Sensor reading frequency
- Maintenance completion rates
- Spare part usage trends

### Logging
- Structured logging with Serilog
- Log levels: Debug, Info, Warning, Error, Critical
- Correlation IDs for request tracking

### Health Checks
```csharp
builder.Services.AddHealthChecks()
    .AddDbContextCheck<EquipTrackDbContext>()
    .AddRabbitMQ(rabbitConnectionString);
```

## Security Considerations

### Authentication & Authorization
- JWT tokens for API access
- Role-based access control (RBAC)
- Technician, Manager, Admin roles

### Data Protection
- Encrypt sensitive data at rest
- Use HTTPS for all communications
- Audit all write operations

## Next Steps

1. **Complete Repository Implementations**
   - Implement all read/write repositories
   - Add EF Core configurations

2. **Complete CQRS Handlers**
   - Implement remaining command handlers
   - Implement remaining query handlers

3. **Add RabbitMQ Services**
   - Implement publishers
   - Implement consumers
   - Configure exchanges and queues

4. **Create API Controllers**
   - MachinesController
   - RobotsController
   - SensorsController
   - LogsController

5. **Add Data Seeding**
   - Seed machines
   - Seed robots
   - Seed sensors
   - Seed initial readings

6. **Implement Caching**
   - Add cache pipeline for queries
   - Configure Redis/Memory cache

7. **Add Integration Tests**
   - API tests
   - Repository tests
   - Message flow tests

8. **Documentation**
   - API documentation (Swagger)
   - Developer guide
   - Deployment guide

## Conclusion

This implementation provides a solid foundation for a production-ready CMMS + IoT system. The architecture follows best practices including:

- ✅ Clean Architecture
- ✅ CQRS Pattern
- ✅ Repository Pattern
- ✅ Domain-Driven Design
- ✅ Event-Driven Architecture
- ✅ Validation Pipeline
- ✅ Comprehensive Logging
- ✅ Real-time Monitoring
- ✅ Scalable Design

The system is designed to handle real-world CMMS scenarios with full auditability, real-time sensor monitoring, and comprehensive asset management capabilities.
