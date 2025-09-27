using Microsoft.AspNetCore.Mvc;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Dashboard.API.Extensions;

public static class ControllerExtensions
{
    public static ActionResult<T> ToActionResult<T>(this Result<T> result)
    {
        return result.Status switch
        {
            ResultStatus.Ok => new OkObjectResult(new ApiResponse<T>
            {
                Success = true,
                Data = result.Value,
                Message = result.SuccessMessage
            }),
            ResultStatus.NotFound => new NotFoundObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "Resource not found",
                Errors = result.Errors
            }),
            ResultStatus.Invalid => new BadRequestObjectResult(new ApiResponse
            {
                Success = false,
                Message = "Validation failed",
                Errors = result.Errors,
                ValidationErrors = result.ValidationErrors.ToDictionary(
                    ve => ve.PropertyName,
                    ve => ve.ErrorMessage)
            }),
            ResultStatus.Unauthorized => new UnauthorizedObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "Unauthorized",
                Errors = result.Errors
            }),
            ResultStatus.Forbidden => new ObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "Forbidden",
                Errors = result.Errors
            })
            {
                StatusCode = 403
            },
            ResultStatus.Conflict => new ConflictObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "Conflict",
                Errors = result.Errors
            }),
            ResultStatus.Error => new ObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "An error occurred",
                Errors = result.Errors
            })
            {
                StatusCode = 500
            },
            ResultStatus.CriticalError => new ObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "A critical error occurred",
                Errors = result.Errors
            })
            {
                StatusCode = 500
            },
            ResultStatus.Unavailable => new ObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "Service unavailable",
                Errors = result.Errors
            })
            {
                StatusCode = 503
            },
            _ => new ObjectResult(new ApiResponse
            {
                Success = false,
                Message = "An unexpected error occurred",
                Errors = result.Errors
            })
            {
                StatusCode = 500
            }
        };
    }

    public static ActionResult ToActionResult(this Result result)
    {
        return result.Status switch
        {
            ResultStatus.Ok => new OkObjectResult(new ApiResponse
            {
                Success = true,
                Message = result.SuccessMessage
            }),
            ResultStatus.NotFound => new NotFoundObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "Resource not found",
                Errors = result.Errors
            }),
            ResultStatus.Invalid => new BadRequestObjectResult(new ApiResponse
            {
                Success = false,
                Message = "Validation failed",
                Errors = result.Errors,
                ValidationErrors = result.ValidationErrors.ToDictionary(
                    ve => ve.PropertyName,
                    ve => ve.ErrorMessage)
            }),
            ResultStatus.Unauthorized => new UnauthorizedObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "Unauthorized",
                Errors = result.Errors
            }),
            ResultStatus.Forbidden => new ObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "Forbidden",
                Errors = result.Errors
            })
            {
                StatusCode = 403
            },
            ResultStatus.Conflict => new ConflictObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "Conflict",
                Errors = result.Errors
            }),
            ResultStatus.Error => new ObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "An error occurred",
                Errors = result.Errors
            })
            {
                StatusCode = 500
            },
            ResultStatus.CriticalError => new ObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "A critical error occurred",
                Errors = result.Errors
            })
            {
                StatusCode = 500
            },
            ResultStatus.Unavailable => new ObjectResult(new ApiResponse
            {
                Success = false,
                Message = result.Errors.FirstOrDefault() ?? "Service unavailable",
                Errors = result.Errors
            })
            {
                StatusCode = 503
            },
            _ => new ObjectResult(new ApiResponse
            {
                Success = false,
                Message = "An unexpected error occurred",
                Errors = result.Errors
            })
            {
                StatusCode = 500
            }
        };
    }

    public static ActionResult<T> ToPaginatedActionResult<T>(this PagedResult<T> result)
    {
        if (!result.IsSuccess)
        {
            return ((Result<T>)result).ToActionResult();
        }

        return new OkObjectResult(new PaginatedApiResponse<T>
        {
            Success = true,
            Data = result.Value,
            Message = result.SuccessMessage,
            Pagination = new PaginationInfo
            {
                CurrentPage = result.PagedInfo.PageNumber,
                PageSize = result.PagedInfo.PageSize,
                TotalCount = result.PagedInfo.TotalCount,
                TotalPages = result.PagedInfo.TotalPages,
                HasPrevious = result.PagedInfo.HasPreviousPage,
                HasNext = result.PagedInfo.HasNextPage
            }
        });
    }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public IEnumerable<string>? Errors { get; set; }
    public Dictionary<string, string>? ValidationErrors { get; set; }
}

public class ApiResponse<T> : ApiResponse
{
    public T? Data { get; set; }
}

public class PaginatedApiResponse<T> : ApiResponse<T>
{
    public PaginationInfo? Pagination { get; set; }
}

public class PaginationInfo
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
}