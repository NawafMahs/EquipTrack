using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using EquipTrack.Core.SharedKernel;

namespace EquipTrack.Query.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginatedList<T>> ToPaginatedListAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<T>(items, totalCount, page, pageSize);
    }

    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    public static IQueryable<T> OrderByDynamic<T>(
        this IQueryable<T> query,
        string? sortBy,
        string? sortDirection = "ASC")
    {
        if (string.IsNullOrWhiteSpace(sortBy))
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(parameter, sortBy);
        var lambda = Expression.Lambda(property, parameter);

        var methodName = sortDirection?.ToUpper() == "DESC" ? "OrderByDescending" : "OrderBy";
        var resultExpression = Expression.Call(
            typeof(Queryable),
            methodName,
            new Type[] { typeof(T), property.Type },
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(resultExpression);
    }

    public static IQueryable<T> SearchText<T>(
        this IQueryable<T> query,
        string? searchTerm,
        params Expression<Func<T, string>>[] searchProperties)
    {
        if (string.IsNullOrWhiteSpace(searchTerm) || !searchProperties.Any())
            return query;

        var parameter = Expression.Parameter(typeof(T), "x");
        Expression? combinedExpression = null;

        foreach (var property in searchProperties)
        {
            var propertyAccess = Expression.Invoke(property, parameter);
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
            var containsExpression = Expression.Call(
                propertyAccess,
                containsMethod!,
                Expression.Constant(searchTerm));

            combinedExpression = combinedExpression == null
                ? containsExpression
                : Expression.OrElse(combinedExpression, containsExpression);
        }

        if (combinedExpression != null)
        {
            var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
            query = query.Where(lambda);
        }

        return query;
    }

    public static IQueryable<T> DateRange<T>(
        this IQueryable<T> query,
        Expression<Func<T, DateTime>> dateProperty,
        DateTime? fromDate,
        DateTime? toDate)
    {
        if (fromDate.HasValue)
        {
            var fromExpression = Expression.Lambda<Func<T, bool>>(
                Expression.GreaterThanOrEqual(
                    Expression.Invoke(dateProperty, dateProperty.Parameters[0]),
                    Expression.Constant(fromDate.Value)),
                dateProperty.Parameters[0]);
            query = query.Where(fromExpression);
        }

        if (toDate.HasValue)
        {
            var toExpression = Expression.Lambda<Func<T, bool>>(
                Expression.LessThanOrEqual(
                    Expression.Invoke(dateProperty, dateProperty.Parameters[0]),
                    Expression.Constant(toDate.Value)),
                dateProperty.Parameters[0]);
            query = query.Where(toExpression);
        }

        return query;
    }

    public static IQueryable<T> DecimalRange<T>(
        this IQueryable<T> query,
        Expression<Func<T, decimal>> property,
        decimal? minValue,
        decimal? maxValue)
    {
        if (minValue.HasValue)
        {
            var minExpression = Expression.Lambda<Func<T, bool>>(
                Expression.GreaterThanOrEqual(
                    Expression.Invoke(property, property.Parameters[0]),
                    Expression.Constant(minValue.Value)),
                property.Parameters[0]);
            query = query.Where(minExpression);
        }

        if (maxValue.HasValue)
        {
            var maxExpression = Expression.Lambda<Func<T, bool>>(
                Expression.LessThanOrEqual(
                    Expression.Invoke(property, property.Parameters[0]),
                    Expression.Constant(maxValue.Value)),
                property.Parameters[0]);
            query = query.Where(maxExpression);
        }

        return query;
    }

    public static IQueryable<T> IntRange<T>(
        this IQueryable<T> query,
        Expression<Func<T, int>> property,
        int? minValue,
        int? maxValue)
    {
        if (minValue.HasValue)
        {
            var minExpression = Expression.Lambda<Func<T, bool>>(
                Expression.GreaterThanOrEqual(
                    Expression.Invoke(property, property.Parameters[0]),
                    Expression.Constant(minValue.Value)),
                property.Parameters[0]);
            query = query.Where(minExpression);
        }

        if (maxValue.HasValue)
        {
            var maxExpression = Expression.Lambda<Func<T, bool>>(
                Expression.LessThanOrEqual(
                    Expression.Invoke(property, property.Parameters[0]),
                    Expression.Constant(maxValue.Value)),
                property.Parameters[0]);
            query = query.Where(maxExpression);
        }

        return query;
    }
}