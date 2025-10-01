using FluentValidation;
using MediatR;
using EquipTrack.Core.SharedKernel;
using Microsoft.Extensions.Logging;

namespace EquipTrack.Application.Common.Behaviors;

/// <summary>
/// Pipeline behavior that validates commands before they are handled.
/// Implements the validator pipeline pattern following NexusCore architecture.
/// </summary>
/// <typeparam name="TRequest">The type of the request (command)</typeparam>
/// <typeparam name="TResponse">The type of the response (Result)</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IResult
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;

    public ValidationBehavior(
        IEnumerable<IValidator<TRequest>> validators,
        ILogger<ValidationBehavior<TRequest, TResponse>> logger)
    {
        _validators = validators ?? throw new ArgumentNullException(nameof(validators));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        // If no validators are registered, continue to next behavior
        if (!_validators.Any())
        {
            _logger.LogDebug("No validators registered for {RequestName}", requestName);
            return await next();
        }

        _logger.LogDebug("Validating {RequestName}", requestName);

        // Run all validators
        var context = new ValidationContext<TRequest>(request);
        
        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Collect all validation failures
        var failures = validationResults
            .Where(r => !r.IsValid)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Any())
        {
            var errors = failures
                .Select(f => $"{f.PropertyName}: {f.ErrorMessage}")
                .ToList();

            _logger.LogWarning(
                "Validation failed for {RequestName}. Errors: {Errors}",
                requestName,
                string.Join("; ", errors));

            // Create a Result.Failure response
            return CreateValidationFailureResponse(errors);
        }

        _logger.LogDebug("Validation succeeded for {RequestName}", requestName);

        // Validation passed, continue to next behavior/handler
        return await next();
    }

    private static TResponse CreateValidationFailureResponse(List<string> errors)
    {
        var errorMessage = string.Join("; ", errors);
        
        // Use reflection to create the appropriate Result type
        var responseType = typeof(TResponse);
        
        // Check if TResponse is Result<T>
        if (responseType.IsGenericType && 
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var genericArg = responseType.GetGenericArguments()[0];
            var resultType = typeof(Result<>).MakeGenericType(genericArg);
            var failureMethod = resultType.GetMethod("Failure", new[] { typeof(string) });
            
            if (failureMethod != null)
            {
                return (TResponse)failureMethod.Invoke(null, new object[] { errorMessage })!;
            }
        }
        
        // Fallback to Result.Failure for non-generic Result
        if (responseType == typeof(Result))
        {
            return (TResponse)(object)Result.Failure(errorMessage);
        }

        throw new InvalidOperationException(
            $"Cannot create validation failure response for type {responseType.Name}");
    }
}


