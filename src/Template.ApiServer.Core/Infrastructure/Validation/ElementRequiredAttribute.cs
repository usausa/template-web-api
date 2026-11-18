namespace Template.ApiServer.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class ElementRequiredAttribute : ValidationAttribute
{
    public bool AllowEmptyStrings { get; set; }

    public ElementRequiredAttribute()
        : base(static () => ValidationMessages.ElementRequired)
    {
    }

    public override bool IsValid(object? value)
    {
        if (value is string str)
        {
            return ValidationHelper.HasValue(str, AllowEmptyStrings);
        }

        if (value is not IEnumerable values)
        {
            return value is not null;
        }

        var count = 0;
        foreach (var element in values)
        {
            if (!ValidationHelper.HasValue(element, AllowEmptyStrings))
            {
                return false;
            }

            count++;
        }

        return count > 0;
    }
}
