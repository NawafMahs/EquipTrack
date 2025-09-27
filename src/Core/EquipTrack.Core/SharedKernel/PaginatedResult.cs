namespace EquipTrack.Core.SharedKernel;

/// <summary>
/// Represents a paginated result containing both the result data and pagination information.
/// </summary>
/// <typeparam name="T">The type of the result data.</typeparam>
public class PagedResult<T> : Result<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PagedResult{T}"/> class.
    /// </summary>
    /// <param name="pagedInfo">The pagination information.</param>
    /// <param name="value">The result value.</param>
    public PagedResult(PaginatedList<T> pagedInfo, T value) : base(value)
    {
        PagedInfo = pagedInfo;
    }

    /// <summary>
    /// Gets the pagination information.
    /// </summary>
    public PaginatedList<T> PagedInfo { get; }
}