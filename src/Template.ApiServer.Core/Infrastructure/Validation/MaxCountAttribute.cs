namespace Template.ApiServer.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class MaxCountAttribute : ValidationAttribute
{
    public int Count { get; }

    public MaxCountAttribute(int count)
        : base(static () => ValidationMessages.MaxCount)
    {
        Count = count;
    }

    public override bool IsValid(object? value)
    {
        if (Count < 0)
        {
            throw new InvalidOperationException($"Invalid count. count=[{Count}]");
        }

        return value switch
        {
            string => true,
            ICollection collection => collection.Count <= Count,
            IEnumerable values => values.Cast<object?>().Count() <= Count,
            _ => true
        };
    }

    public override string FormatMessage(string format, string name) =>
        String.Format(CultureInfo.CurrentCulture, format, name, Count);
}
