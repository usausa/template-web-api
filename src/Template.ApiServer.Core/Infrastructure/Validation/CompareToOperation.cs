namespace Template.ApiServer.Infrastructure.Validation;

public enum CompareToOperation
{
    LessThan,
    LessThanOrEqual,
    GreaterThan,
    GreaterThanOrEqual
}

internal static class CompareToOperationExtensions
{
    public static bool IsValid(this CompareToOperation operation, object? value, object? other)
    {
        if ((value is not IComparable comparable) || (other is null))
        {
            return true;
        }

        var compare = comparable.CompareTo(other);
        return operation switch
        {
            CompareToOperation.LessThan => compare < 0,
            CompareToOperation.LessThanOrEqual => compare <= 0,
            CompareToOperation.GreaterThan => compare > 0,
            CompareToOperation.GreaterThanOrEqual => compare >= 0,
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };
    }

    public static string ResolveMessage(this CompareToOperation operation) =>
        operation switch
        {
            CompareToOperation.LessThan => ValidationMessages.LessThan,
            CompareToOperation.LessThanOrEqual => ValidationMessages.LessThanOrEqual,
            CompareToOperation.GreaterThan => ValidationMessages.GreaterThan,
            CompareToOperation.GreaterThanOrEqual => ValidationMessages.GreaterThanOrEqual,
            _ => throw new ArgumentOutOfRangeException(nameof(operation))
        };
}
