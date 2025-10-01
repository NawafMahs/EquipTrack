# Domain Model Refactoring Plan - DDD with CQRS

## Overview
Refactor the CMMS domain model to follow Domain-Driven Design principles with a unified Asset entity using Table-Per-Hierarchy (TPH) inheritance pattern.

## Current Issues
1. **Duplication**: Separate `Machine`, `Robot`, and `Asset` entities with overlapping properties
2. **Inconsistency**: Different status enums (MachineStatus, RobotStatus, AssetStatus)
3. **Complexity**: Multiple log tables (MachineLog, RobotLog) and sensor tables
4. **Scalability**: Adding new asset types requires new entities and tables

## Proposed Solution

### 1. Unified Asset Entity (Base Class)
```
Asset (Abstract Base)
├── Id: Guid
├── Name: string
├── Description: string
├── SerialNumber: string
├── Model: string
├── Manufacturer: string
├── AssetTag: string
├── AssetType: AssetType (enum/discriminator)
├── Status: AssetStatus
├── Location: string
├── InstallationDate: DateTime
├── PurchaseDate: DateTime
├── PurchasePrice: decimal
├── Criticality: AssetCriticality
├── WarrantyExpiryDate: DateTime?
├── OperatingHours: int
├── LastMaintenanceDate: DateTime?
├── NextMaintenanceDate: DateTime?
├── IsActive: bool
├── Notes: string?
├── ImageUrl: string?
└── Metadata: Dictionary<string, string> (for extensibility)
```

### 2. Specialized Asset Types (Derived Classes)

#### Machine (extends Asset)
- PowerRating: decimal?
- VoltageRequirement: string?
- MachineTypeRef: string?
- CurrentEfficiency: decimal?

#### Robot (extends Asset)
- RobotType: RobotType
- MaxPayloadKg: int
- ReachMeters: decimal
- BatteryLevel: decimal?
- CurrentTask: string?
- CycleCount: int
- FirmwareVersion: string?

#### Sensor (extends Asset)
- SensorType: SensorType
- MeasurementUnit: string
- MinValue: decimal?
- MaxValue: decimal?
- CalibrationDate: DateTime?

#### Vehicle (extends Asset)
- VehicleType: string
- LicensePlate: string?
- Mileage: int
- FuelType: string?

### 3. Unified Supporting Entities

#### AssetLog (replaces MachineLog, RobotLog)
```
AssetLog
├── Id: Guid
├── AssetId: Guid (FK)
├── LogType: AssetLogType
├── Severity: LogSeverity
├── Message: string
├── Timestamp: DateTime
├── Source: string? (RabbitMQ, MQTT, Manual)
├── Metadata: Dictionary<string, object>
└── Asset: Asset (navigation)
```

#### AssetSensor (replaces MachineSensor, RobotSensor)
```
AssetSensor
├── Id: Guid
├── AssetId: Guid (FK)
├── SensorId: Guid (FK to Asset where AssetType = Sensor)
├── MountLocation: string
├── IsActive: bool
└── Asset: Asset (navigation)
```

#### SensorReading
```
SensorReading
├── Id: Guid
├── SensorId: Guid (FK to Asset)
├── Value: decimal
├── Unit: string
├── Timestamp: DateTime
├── Quality: ReadingQuality
└── Sensor: Asset (navigation)
```

#### AssetSparePartUsage (replaces MachineSparePartUsage, RobotSparePartUsage)
```
AssetSparePartUsage
├── Id: Guid
├── AssetId: Guid (FK)
├── SparePartId: Guid (FK)
├── Quantity: int
├── UsageDate: DateTime
├── WorkOrderId: Guid? (FK)
├── Notes: string?
├── Asset: Asset (navigation)
├── SparePart: SparePart (navigation)
└── WorkOrder: WorkOrder? (navigation)
```

### 4. Enums Consolidation

#### AssetType (Discriminator)
```csharp
public enum AssetType
{
    Generic = 0,
    Machine = 1,
    Robot = 2,
    Sensor = 3,
    Vehicle = 4,
    Conveyor = 5,
    HVAC = 6,
    Pump = 7,
    Compressor = 8,
    Generator = 9,
    Tool = 10
}
```

#### AssetStatus (Unified)
```csharp
public enum AssetStatus
{
    Active = 0,
    Idle = 1,
    Running = 2,
    UnderMaintenance = 3,
    Error = 4,
    OutOfService = 5,
    Charging = 6,  // For robots/electric vehicles
    Disposed = 7,
    Inactive = 8
}
```

#### AssetLogType
```csharp
public enum AssetLogType
{
    StatusChange = 0,
    MaintenancePerformed = 1,
    ErrorOccurred = 2,
    WarningIssued = 3,
    ConfigurationChanged = 4,
    SensorAlert = 5,
    BatteryAlert = 6,
    PerformanceAlert = 7,
    RabbitMQMessage = 8,
    MQTTMessage = 9,
    ManualEntry = 10
}
```

### 5. EF Core Configuration

#### Table-Per-Hierarchy (TPH) Strategy
```csharp
modelBuilder.Entity<Asset>()
    .HasDiscriminator<AssetType>("AssetType")
    .HasValue<Asset>(AssetType.Generic)
    .HasValue<Machine>(AssetType.Machine)
    .HasValue<Robot>(AssetType.Robot)
    .HasValue<Sensor>(AssetType.Sensor)
    .HasValue<Vehicle>(AssetType.Vehicle);
```

### 6. CQRS Commands & Queries

#### Commands
- `RegisterAssetCommand` - Create new asset
- `UpdateAssetCommand` - Update asset details
- `ChangeAssetStatusCommand` - Change asset status
- `RecordMaintenanceCommand` - Record maintenance activity
- `AssignTaskToRobotCommand` - Assign task (robot-specific)
- `UpdateBatteryLevelCommand` - Update battery (robot-specific)
- `RecordSensorReadingCommand` - Record sensor data
- `DeactivateAssetCommand` - Deactivate asset
- `DisposeAssetCommand` - Dispose asset

#### Queries
- `GetAssetsByTypeQuery` - Get assets by type
- `GetAssetByIdQuery` - Get single asset
- `GetAssetWithLogsQuery` - Get asset with logs
- `GetAssetWithMaintenanceHistoryQuery` - Get asset with maintenance
- `GetAssetsRequiringMaintenanceQuery` - Get assets needing maintenance
- `GetAssetsByCriticalityQuery` - Get assets by criticality
- `GetAssetsByLocationQuery` - Get assets by location
- `GetRobotsWithLowBatteryQuery` - Get robots with low battery
- `GetAssetPerformanceMetricsQuery` - Get performance data

### 7. Repository Pattern

#### Read Repository
```csharp
public interface IAssetReadOnlyRepository : IReadOnlyRepository<Asset>
{
    Task<Asset?> GetByIdWithLogsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Asset?> GetBySerialNumberAsync(string serialNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Asset>> GetByTypeAsync(AssetType assetType, CancellationToken cancellationToken = default);
    Task<IEnumerable<Asset>> GetByLocationAsync(string location, CancellationToken cancellationToken = default);
    Task<IEnumerable<Asset>> GetRequiringMaintenanceAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Robot>> GetRobotsWithLowBatteryAsync(decimal threshold, CancellationToken cancellationToken = default);
}
```

#### Write Repository
```csharp
public interface IAssetWriteOnlyRepository : IWriteOnlyRepository<Asset>
{
    Task<Asset> AddAsync(Asset asset, CancellationToken cancellationToken = default);
    Task UpdateAsync(Asset asset, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
```

### 8. Migration Strategy

#### Phase 1: Create New Schema
1. Create new `Asset` table with discriminator
2. Create new `AssetLog` table
3. Create new `AssetSensor` table
4. Create new `AssetSparePartUsage` table

#### Phase 2: Data Migration
1. Migrate `Machine` data to `Asset` (AssetType = Machine)
2. Migrate `Robot` data to `Asset` (AssetType = Robot)
3. Migrate existing `Asset` data (AssetType = Generic)
4. Migrate logs and sensor data
5. Update foreign keys in `WorkOrder` table

#### Phase 3: Cleanup
1. Drop old tables: `Machine`, `Robot`, `MachineLog`, `RobotLog`, etc.
2. Update all queries and commands
3. Update controllers and services

### 9. Benefits

1. **Single Source of Truth**: One Asset table for all equipment
2. **Extensibility**: Easy to add new asset types (Conveyor, HVAC, etc.)
3. **Consistency**: Unified status management and logging
4. **Simplified Queries**: Query all assets or filter by type
5. **Reduced Duplication**: Shared properties and behaviors
6. **Better Reporting**: Cross-asset analytics and reporting
7. **Flexible Metadata**: JSON column for type-specific attributes

### 10. Example Usage

```csharp
// Register a new robot
var command = new RegisterAssetCommand
{
    Name = "Robot-001",
    AssetType = AssetType.Robot,
    SerialNumber = "RBT-12345",
    Model = "ABB IRB 6700",
    Manufacturer = "ABB",
    Location = "Assembly Line 1",
    RobotSpecificData = new RobotData
    {
        RobotType = RobotType.Industrial,
        MaxPayloadKg = 150,
        ReachMeters = 2.6m,
        FirmwareVersion = "v3.2.1"
    }
};

// Query robots with low battery
var query = new GetRobotsWithLowBatteryQuery { Threshold = 20 };
var robots = await mediator.Send(query);

// Get all assets requiring maintenance
var maintenanceQuery = new GetAssetsRequiringMaintenanceQuery();
var assetsNeedingMaintenance = await mediator.Send(maintenanceQuery);
```

## Implementation Timeline

1. **Week 1**: Create new domain entities and enums
2. **Week 2**: Implement EF Core configurations and migrations
3. **Week 3**: Create CQRS commands and queries
4. **Week 4**: Update repositories and services
5. **Week 5**: Data migration scripts
6. **Week 6**: Update controllers and API
7. **Week 7**: Testing and validation
8. **Week 8**: Cleanup and documentation
