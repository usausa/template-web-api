namespace Template.ApiServer.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed class Ms932LengthAttribute : ValidationAttribute
{
    private static readonly Encoding Ms932 = CodePagesEncodingProvider.Instance.GetEncoding(932)!;

    public int MinimumLength { get; }

    public int MaximumLength { get; }

    public Ms932LengthAttribute(int maximumLength)
        : this(0, maximumLength)
    {
    }

    public Ms932LengthAttribute(int minimumLength, int maximumLength)
        : base(() => ResolveMessage(minimumLength, maximumLength))
    {
        MinimumLength = minimumLength;
        MaximumLength = maximumLength;
    }

    public override bool IsValid(object? value)
    {
        EnsureLegalLengths();

        return value switch
        {
            string str => IsValidLength(str),
            IEnumerable<string?> values => values.All(x => (x is null) || IsValidLength(x)),
            _ => true
        };
    }

    public override string FormatMessage(string format, string name) =>
        String.Format(CultureInfo.CurrentCulture, format, name, MaximumLength, MinimumLength);

    private bool IsValidLength(string value)
    {
        var length = Ms932.GetByteCount(value);
        return (length >= MinimumLength) && (length <= MaximumLength);
    }

    private void EnsureLegalLengths()
    {
        if ((MinimumLength < 0) || (MaximumLength < MinimumLength))
        {
            throw new InvalidOperationException($"Invalid length. minimum=[{MinimumLength}], maximum=[{MaximumLength}]");
        }
    }

    private static string ResolveMessage(int minimumLength, int maximumLength) =>
        minimumLength == 0
            ? ValidationMessages.Ms932MaxLength
            : minimumLength == maximumLength
                ? ValidationMessages.Ms932FixedLength
                : ValidationMessages.Ms932Length;
}
