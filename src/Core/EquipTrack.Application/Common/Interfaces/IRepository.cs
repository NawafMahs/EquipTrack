// This file has been removed as part of the clean architecture refactoring.
// Generic repository pattern has been replaced with specific read-only and write-only repositories.
// Each entity now has its own dedicated repository interface and implementation.
// This approach follows SOLID principles and provides better testability and maintainability.
//
// For read operations, use IReadOnlyRepository<TEntity, TKey> from EquipTrack.Domain.Common
// For write operations, use IWriteOnlyRepository<TEntity, TKey> from EquipTrack.Domain.Common
// For transaction management, use IUnitOfWork from EquipTrack.Domain.Common