namespace Template.ApiServer.Infrastructure.Validation;

[AttributeUsage(AttributeTargets.Property)]
public sealed class CompareToAttribute : ValidationAttribute
{
    public CompareToOperation Operation { get; }

    public string OtherProperty { get; }

    public override bool RequiresValidationContext => true;

    public CompareToAttribute(CompareToOperation operation, string otherProperty)
        : base(() => operation.ResolveMessage())
    {
        Operation = operation;
        OtherProperty = otherProperty;
    }

    public override string FormatMessage(string format, string name) =>
        String.Format(CultureInfo.CurrentCulture, format, name, OtherProperty);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) =>
        Operation.IsValid(value, ValidationHelper.GetValue(validationContext.ObjectInstance, OtherProperty))
            ? ValidationResult.Success
            : new ValidationResult(FormatErrorMessage(validationContext.DisplayName), validationContext.ToMemberNames());
}
