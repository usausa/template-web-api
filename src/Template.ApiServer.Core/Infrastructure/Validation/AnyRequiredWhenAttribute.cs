namespace Template.ApiServer.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
public sealed class AnyRequiredWhenAttribute : ConditionalValidationAttribute
{
    private readonly string[] properties;

    public IReadOnlyList<string> Properties => properties;

    public bool AllowEmptyStrings { get; set; }

    public override object TypeId => this;

    public AnyRequiredWhenAttribute(string conditionProperty, params string[] properties)
        : base(conditionProperty, static () => ValidationMessages.AnyRequired)
    {
        this.properties = properties;
    }

    public override string FormatMessage(string format, string name) =>
        String.Format(CultureInfo.CurrentCulture, format, name, String.Join(", ", properties));

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) =>
        (value is null) || !IsConditionMet(value) || properties.Any(x => ValidationHelper.HasValue(ValidationHelper.GetValue(value, x), AllowEmptyStrings))
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName), properties);
}
