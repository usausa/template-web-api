namespace Template.ApiServer.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class AnyRequiredAttribute : ValidationAttribute
{
    private readonly string[] properties;

    public IReadOnlyList<string> Properties => properties;

    public bool AllowEmptyStrings { get; set; }

    public override bool RequiresValidationContext => true;

    public override object TypeId => this;

    public AnyRequiredAttribute(params string[] properties)
        : base(static () => ValidationMessages.AnyRequired)
    {
        this.properties = properties;
    }

    public override string FormatMessage(string format, string name) =>
        String.Format(CultureInfo.CurrentCulture, format, name, String.Join(", ", properties));

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) =>
        (value is null) || properties.Any(x => ValidationHelper.HasValue(ValidationHelper.GetValue(value, x), AllowEmptyStrings))
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName), properties);
}
