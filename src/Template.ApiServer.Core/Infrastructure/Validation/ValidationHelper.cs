namespace Template.ApiServer.Infrastructure.Validation;

using BunnyTail.MemberAccessor;

internal static class ValidationHelper
{
    public static object? GetValue(object instance, string name)
    {
        var type = instance.GetType();
        var accessor = AccessorProvider.FindAccessor(type);
        if (accessor is null)
        {
            throw new InvalidOperationException($"Accessor is not supported. type=[{type.FullName}]");
        }

        return accessor.GetValue(instance, name);
    }

    public static bool HasValue(object? value, bool allowEmptyStrings) =>
        (value is not null) && (allowEmptyStrings || (value is not string str) || !String.IsNullOrWhiteSpace(str));

    public static string[]? ToMemberNames(this ValidationContext validationContext) =>
        validationContext.MemberName is not null ? [validationContext.MemberName] : null;
}
