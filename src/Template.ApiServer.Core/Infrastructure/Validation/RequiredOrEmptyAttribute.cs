namespace Template.ApiServer.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredOrEmptyAttribute : ConditionalValidationAttribute
{
    public bool AllowEmptyStrings { get; set; }

    public string? EmptyErrorMessage { get; set; }

    public RequiredOrEmptyAttribute(string conditionProperty)
        : base(conditionProperty, static () => ValidationMessages.Required)
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var hasValue = ValidationHelper.HasValue(value, AllowEmptyStrings);
        return GetConditionValue(validationContext.ObjectInstance) switch
        {
            true when !hasValue => new ValidationResult(FormatErrorMessage(validationContext.DisplayName), validationContext.ToMemberNames()),
            false when hasValue => new ValidationResult(String.Format(CultureInfo.CurrentCulture, EmptyErrorMessage ?? ValidationMessages.Empty, validationContext.DisplayName), validationContext.ToMemberNames()),
            _ => ValidationResult.Success
        };
    }
}
