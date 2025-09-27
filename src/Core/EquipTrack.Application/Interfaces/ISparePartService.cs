using EquipTrack.Application.DTOs;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Application.Interfaces;

public interface ISparePartService
{
    Task<Result<IEnumerable<SparePartQuery>>> GetAllSparePartsAsync(CancellationToken cancellationToken = default);
    Task<Result<SparePartQuery>> GetSparePartByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<SparePartQuery>>> GetSparePartsByCategoryAsync(string category, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<SparePartQuery>>> GetLowStockSparePartsAsync(CancellationToken cancellationToken = default);
    Task<Result<SparePartQuery>> CreateSparePartAsync(CreateSparePartCommand createSparePartDto, CancellationToken cancellationToken = default);
    Task<Result<SparePartQuery>> UpdateSparePartAsync(Guid id, UpdateSparePartCommand updateSparePartDto, CancellationToken cancellationToken = default);
    Task<Result> DeleteSparePartAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Result> UpdateStockAsync(Guid id, UpdateStockCommand updateStockDto, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<SparePartQuery>>> SearchSparePartsAsync(string searchTerm, CancellationToken cancellationToken = default);
}