using System.Text.Json.Serialization;

namespace EquipTrack.Core.SharedKernel;

public class Result : Result<Result>
{
    public Result()
    {
    }

    protected internal Result(ResultStatus status)
        : base(status)
    {
    }

    public static Result Success()
    {
        return new Result();
    }

    public static Result SuccessWithMessage(string successMessage)
    {
        return new Result
        {
            SuccessMessage = successMessage
        };
    }

    public static Result<T> Success<T>(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Success<T>(T value, string successMessage)
    {
        return new Result<T>(value, successMessage);
    }

    public new static Result Error(params string[] errorMessages)
    {
        return new Result(ResultStatus.Error)
        {
            Errors = errorMessages
        };
    }

    public static Result ErrorWithCorrelationId(string correlationId, params string[] errorMessages)
    {
        return new Result(ResultStatus.Error)
        {
            CorrelationId = correlationId,
            Errors = errorMessages
        };
    }

    public new static Result Invalid(ValidationError validationError)
    {
        return new Result(ResultStatus.Invalid)
        {
            ValidationErrors = { validationError }
        };
    }

    public new static Result Invalid(params ValidationError[] validationErrors)
    {
        return new Result(ResultStatus.Invalid)
        {
            ValidationErrors = new List<ValidationError>(validationErrors)
        };
    }

    public new static Result Invalid(List<ValidationError> validationErrors)
    {
        return new Result(ResultStatus.Invalid)
        {
            ValidationErrors = validationErrors
        };
    }

    public new static Result NotFound()
    {
        return new Result(ResultStatus.NotFound);
    }

    public new static Result NotFound(params string[] errorMessages)
    {
        return new Result(ResultStatus.NotFound)
        {
            Errors = errorMessages
        };
    }

    public new static Result Forbidden()
    {
        return new Result(ResultStatus.Forbidden);
    }

    public new static Result Unauthorized()
    {
        return new Result(ResultStatus.Unauthorized);
    }

    public new static Result Unauthorized(params string[] errorMessages)
    {
        return new Result(ResultStatus.Unauthorized)
        {
            Errors = errorMessages
        };
    }

    public new static Result Conflict()
    {
        return new Result(ResultStatus.Conflict);
    }

    public new static Result Conflict(params string[] errorMessages)
    {
        return new Result(ResultStatus.Conflict)
        {
            Errors = errorMessages
        };
    }

    public new static Result Unavailable(params string[] errorMessages)
    {
        return new Result(ResultStatus.Unavailable)
        {
            Errors = errorMessages
        };
    }

    public new static Result CriticalError(params string[] errorMessages)
    {
        return new Result(ResultStatus.CriticalError)
        {
            Errors = errorMessages
        };
    }
}

public class Result<T> : IResult
{
    public T Value { get; }

    [JsonIgnore]
    public Type ValueType => typeof(T);

    public ResultStatus Status { get; protected set; }

    public bool IsSuccess => Status == ResultStatus.Ok;

    public string SuccessMessage { get; protected set; } = string.Empty;

    public string CorrelationId { get; protected set; } = string.Empty;

    public IEnumerable<string> Errors { get; protected set; } = new List<string>();

    public List<ValidationError> ValidationErrors { get; protected set; } = new List<ValidationError>();

    public IDictionary<string, List<string>> PropertyErrors { get; private set; } = new Dictionary<string, List<string>>();

    protected Result()
    {
    }

    public Result(T value)
    {
        Value = value;
    }

    protected internal Result(T value, string successMessage)
        : this(value)
    {
        SuccessMessage = successMessage;
    }

    protected Result(ResultStatus status)
    {
        Status = status;
    }

    public static implicit operator T(Result<T> result)
    {
        return result.Value;
    }

    public static implicit operator Result<T>(T value)
    {
        return new Result<T>(value);
    }

    public static implicit operator Result<T>(Result result)
    {
        return new Result<T>(default(T))
        {
            Status = result.Status,
            Errors = result.Errors,
            SuccessMessage = result.SuccessMessage,
            CorrelationId = result.CorrelationId,
            ValidationErrors = result.ValidationErrors
        };
    }

    public object GetValue()
    {
        return Value;
    }

    public PagedResult<T> ToPagedResult(PaginatedList<T> pagedInfo)
    {
        return new PagedResult<T>(pagedInfo, Value)
        {
            Status = Status,
            SuccessMessage = SuccessMessage,
            CorrelationId = CorrelationId,
            Errors = Errors,
            ValidationErrors = ValidationErrors
        };
    }

    public static Result<T> Success(T value)
    {
        return new Result<T>(value);
    }

    public static Result<T> Success(T value, string successMessage)
    {
        return new Result<T>(value, successMessage);
    }

    public static Result<T> Error(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.Error)
        {
            Errors = errorMessages
        };
    }

    public void AddPropertyError(string errorMessage, string propertyName)
    {
        if (!PropertyErrors.ContainsKey(propertyName))
        {
            PropertyErrors[propertyName] = new List<string>();
        }
        PropertyErrors[propertyName].Add(errorMessage);
    }

    public static Result<T> Error(string errorMessage, string propertyName)
    {
        var result = new Result<T>(ResultStatus.Error);
        result.AddPropertyError(errorMessage, propertyName);
        return result;
    }

    public static Result<T> Invalid(ValidationError validationError)
    {
        return new Result<T>(ResultStatus.Invalid)
        {
            ValidationErrors = { validationError }
        };
    }

    public static Result<T> Invalid(params ValidationError[] validationErrors)
    {
        return new Result<T>(ResultStatus.Invalid)
        {
            ValidationErrors = new List<ValidationError>(validationErrors)
        };
    }

    public static Result<T> Invalid(List<ValidationError> validationErrors)
    {
        return new Result<T>(ResultStatus.Invalid)
        {
            ValidationErrors = validationErrors
        };
    }

    public static Result<T> NotFound()
    {
        return new Result<T>(ResultStatus.NotFound);
    }

    public static Result<T> NotFound(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.NotFound)
        {
            Errors = errorMessages
        };
    }

    public static Result<T> Forbidden()
    {
        return new Result<T>(ResultStatus.Forbidden);
    }

    public static Result<T> Unauthorized()
    {
        return new Result<T>(ResultStatus.Unauthorized);
    }

    public static Result<T> Unauthorized(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.Unauthorized)
        {
            Errors = errorMessages
        };
    }

    public static Result<T> Conflict()
    {
        return new Result<T>(ResultStatus.Conflict);
    }

    public static Result<T> Conflict(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.Conflict)
        {
            Errors = errorMessages
        };
    }

    public static Result<T> CriticalError(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.CriticalError)
        {
            Errors = errorMessages
        };
    }

    public static Result<T> Unavailable(params string[] errorMessages)
    {
        return new Result<T>(ResultStatus.Unavailable)
        {
            Errors = errorMessages
        };
    }
}