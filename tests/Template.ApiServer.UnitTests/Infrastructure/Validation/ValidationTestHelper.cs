namespace Template.ApiServer.Infrastructure.Validation;

using System.ComponentModel.DataAnnotations;

internal static class ValidationTestHelper
{
    public static List<ValidationResult> Validate(object instance)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(instance, new ValidationContext(instance), results, validateAllProperties: true);
        return results;
    }

    public static ValidationResult? ValidateProperty(ValidationAttribute attribute, object instance, string name, object? value) =>
        attribute.GetValidationResult(value, new ValidationContext(instance, name, null, null) { MemberName = name });
}
