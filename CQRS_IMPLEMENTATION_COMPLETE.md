# CQRS with MediatR Implementation - Complete

## Overview

This document outlines the complete CQRS (Command Query Responsibility Segregation) implementation using MediatR for the EquipTrack CMMS system, following the NexusCore project patterns and conventions.

## Architecture Overview

### CQRS Pattern Implementation

The implementation follows a clean separation between:
- **Commands**: Handle Create, Update, Delete operations (CUD)
- **Queries**: Handle Read operations (R)
- **Handlers**: Process commands and queries
- **DTOs**: Data transfer objects with proper naming conventions

### Naming Conventions (Following NexusCore Patterns)

#### Commands
- **Pattern**: `{Entity}{Action}Command`
- **Examples**: 
  - `CreateAssetCommand`
  - `UpdateAssetCommand`
  - `UpdateAssetStatusCommand`
  - `DeleteAssetCommand`

#### Queries
- **Pattern**: `Get{Entity}Query` or `Get{Entity}{Specification}Query`
- **Examples**:
  - `GetAssetByIdQuery`
  - `GetAssetsQuery`
  - `GetAssetsByLocationQuery`

#### Handlers
- **Pattern**: `{Command/Query}Handler`
- **Examples**:
  - `CreateAssetCommandHandler`
  - `GetAssetByIdQueryHandler`

#### DTOs
- **Command DTOs**: `{Entity}{Action}CommandDto`
  - `CreateAssetCommandDto`
  - `UpdateAssetCommandDto`
  - `UpdateAssetStatusCommandDto`
  - `DeleteAssetCommandDto`

- **Query DTOs**: `{Entity}QueryDto`
  - `AssetQueryDto` (for query results)
  - `GetAssetsQueryDto` (for query parameters)
  - `GetAssetsByLocationQueryDto`

## Implementation Details

### 1. Commands (CUD Operations)

#### CreateAssetCommand
```csharp
public sealed record CreateAssetCommand : IRequest<Result<Guid>>
{
    // Properties for asset creation
    public string Name { get; init; }
    public string Description { get; init; }
    public string AssetTag { get; init; }
    public string SerialNumber { get; init; }
    public string Manufacturer { get; init; }
    public string Model { get; init; }
    public string Location { get; init; }
    public AssetCriticality Criticality { get; init; }
    // ... additional properties
}
```

#### Command Handlers
- Use `IWriteOnlyRepository` for data modifications
- Implement the Result pattern for consistent error handling
- Include comprehensive validation using FluentValidation
- Return appropriate result types (`Result<Guid>` for creates, `Result<bool>` for deletes)

### 2. Queries (Read Operations)

#### GetAssetsQuery
```csharp
public sealed record GetAssetsQuery : IRequest<Result<PaginatedList<AssetQueryDto>>>
{
    // Pagination and filtering properties
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SearchTerm { get; init; }
    public AssetStatus? Status { get; init; }
    public AssetCriticality? Criticality { get; init; }
    // ... additional filters
}
```

#### Query Handlers
- Use `IReadOnlyRepository` for data retrieval
- Return DTOs optimized for read operations
- Support pagination, filtering, and sorting
- Implement comprehensive error handling

### 3. Repository Pattern

#### Read-Only Repository
```csharp
public interface IAssetReadOnlyRepository : IReadOnlyRepository<Asset, Guid>
{
    Task<bool> ExistsByAssetTagAsync(string assetTag, Guid? excludeId = null);
    Task<bool> ExistsBySerialNumberAsync(string serialNumber, Guid? excludeId = null);
    Task<PaginatedList<Asset>> GetPagedAsync(/* parameters */);
    // ... additional read methods
}
```

#### Write-Only Repository
```csharp
public interface IAssetWriteOnlyRepository : IWriteOnlyRepository<Asset, Guid>
{
    // Inherits standard CRUD operations
    // GetByIdAsync is included for update operations
}
```

### 4. Controllers

Controllers follow the NexusCore pattern:
- Use MediatR to send commands and queries
- Return appropriate HTTP status codes
- Maintain consistent API response format
- Support both new QueryDto and legacy AssetDto for backward compatibility

```csharp
[HttpGet]
public async Task<IActionResult> GetAssets(/* query parameters */)
{
    var query = new GetAssetsQuery(/* parameters */);
    var result = await _mediator.Send<Result<PaginatedList<AssetQueryDto>>>(query);
    
    if (!result.IsSuccess)
        return BadRequest(result.Errors);
        
    return Ok(result.Value);
}
```

### 5. Validation

Each command and query has its own validator:
- `CreateAssetCommandValidator`
- `UpdateAssetCommandValidator`
- `GetAssetsQueryValidator`
- etc.

Validators use FluentValidation and are automatically registered via dependency injection.

### 6. Mapping

AutoMapper profiles handle entity-to-DTO mapping:
```csharp
// Asset Query mappings
CreateMap<Asset, AssetQueryDto>()
    .ForMember(dest => dest.IsUnderWarranty, opt => opt.MapFrom(src => src.IsUnderWarranty()))
    .ForMember(dest => dest.IsOperational, opt => opt.MapFrom(src => src.IsOperational()));

// Backward compatibility
CreateMap<Asset, AssetDto>()
    .ForMember(dest => dest.IsUnderWarranty, opt => opt.MapFrom(src => src.IsUnderWarranty()))
    .ForMember(dest => dest.IsOperational, opt => opt.MapFrom(src => src.IsOperational()));
```

## Key Features

### 1. Result Pattern
All operations return `Result<T>` or `Result` for consistent error handling:
- `Result<Guid>` for create operations
- `Result<bool>` for delete operations
- `Result<AssetQueryDto>` for single entity queries
- `Result<PaginatedList<AssetQueryDto>>` for paginated queries

### 2. Validation
- FluentValidation for all commands and queries
- Comprehensive business rule validation
- Consistent error message format

### 3. Domain Events
The domain model supports domain events through the AggregateRoot base class:
- `AssetCreatedEvent`
- `AssetStatusChangedEvent`

### 4. Pagination
Built-in pagination support with:
- `PaginatedList<T>` wrapper
- Configurable page size and number
- Total count and navigation information

### 5. Filtering and Sorting
Advanced filtering capabilities:
- Search by multiple fields
- Filter by status, criticality, location, manufacturer
- Configurable sorting with direction

## Benefits

1. **Separation of Concerns**: Clear separation between read and write operations
2. **Scalability**: Independent scaling of read and write sides
3. **Maintainability**: Clean, focused handlers for each operation
4. **Testability**: Easy to unit test individual handlers
5. **Consistency**: Standardized patterns across all operations
6. **Performance**: Optimized queries for read operations
7. **Validation**: Comprehensive validation at the command/query level

## Migration Path

For existing code:
1. **Legacy Support**: Old `AssetDto` is maintained for backward compatibility
2. **Gradual Migration**: New features use CQRS pattern
3. **Service Layer**: Marked as deprecated, redirects to CQRS operations
4. **API Compatibility**: Controllers support both old and new patterns

## Usage Examples

### Creating an Asset
```csharp
var command = new CreateAssetCommand(
    name: "Pump #1",
    description: "Main water pump",
    assetTag: "PUMP-001",
    serialNumber: "SN123456",
    manufacturer: "Acme Corp",
    model: "Model X",
    location: "Building A",
    criticality: AssetCriticality.High
);

var result = await _mediator.Send(command);
if (result.IsSuccess)
{
    var assetId = result.Value;
    // Handle success
}
```

### Querying Assets
```csharp
var query = new GetAssetsQuery(
    pageNumber: 1,
    pageSize: 20,
    searchTerm: "pump",
    status: AssetStatus.Operational,
    criticality: AssetCriticality.High
);

var result = await _mediator.Send(query);
if (result.IsSuccess)
{
    var assets = result.Value;
    // Handle paginated results
}
```

## Conclusion

This CQRS implementation provides a robust, scalable, and maintainable architecture for the EquipTrack CMMS system. It follows industry best practices and the established patterns from the NexusCore project, ensuring consistency and reliability across the application.