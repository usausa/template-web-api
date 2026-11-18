namespace Template.ApiServer.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property)]
public sealed class RequiredWhenAttribute : ConditionalValidationAttribute
{
    public bool AllowEmptyStrings { get; set; }

    public RequiredWhenAttribute(string conditionProperty)
        : base(conditionProperty, static () => ValidationMessages.Required)
    {
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) =>
        !IsConditionMet(validationContext.ObjectInstance) || ValidationHelper.HasValue(value, AllowEmptyStrings)
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName), validationContext.ToMemberNames());
}
