namespace Template.ApiServer.Infrastructure.Validation;

public abstract class ConditionalValidationAttribute : ValidationAttribute
{
    public string ConditionProperty { get; }

    public override bool RequiresValidationContext => true;

    protected ConditionalValidationAttribute(string conditionProperty, Func<string> errorMessageAccessor)
        : base(errorMessageAccessor)
    {
        ConditionProperty = conditionProperty;
    }

    protected object? GetConditionValue(object instance) =>
        ValidationHelper.GetValue(instance, ConditionProperty);

    protected bool IsConditionMet(object instance) =>
        GetConditionValue(instance) is true;
}
