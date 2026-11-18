namespace Template.ApiServer.Infrastructure.Validation;

public static class ValidationMessages
{
    public const string Required = "The {0} field is required.";

    public const string Empty = "The {0} field must be empty.";

    public const string AnyRequired = "At least one of the fields {1} is required.";

    public const string Ms932MaxLength = "The field {0} must be a string with a maximum length of {1} bytes in Shift_JIS.";

    public const string Ms932Length = "The field {0} must be a string with a minimum length of {2} bytes and a maximum length of {1} bytes in Shift_JIS.";

    public const string Ms932FixedLength = "The field {0} must be a string with a length of {1} bytes in Shift_JIS.";

    public const string NoHalfWidthKana = "The field {0} must not contain half-width katakana.";

    public const string FullWidth = "The field {0} must contain only full-width characters.";

    public const string UpperAlphanumeric = "The field {0} must contain only uppercase letters and digits.";

    public const string UpperAlphanumericHyphen = "The field {0} must contain only uppercase letters, digits, and hyphens.";

    public const string Alphanumeric = "The field {0} must contain only letters and digits.";

    public const string AlphanumericHyphen = "The field {0} must contain only letters, digits, and hyphens.";

    public const string Numeric = "The field {0} must contain only digits.";

    public const string NumericHyphen = "The field {0} must contain only digits and hyphens.";

    public const string LessThan = "The field {0} must be less than {1}.";

    public const string LessThanOrEqual = "The field {0} must be less than or equal to {1}.";

    public const string GreaterThan = "The field {0} must be greater than {1}.";

    public const string GreaterThanOrEqual = "The field {0} must be greater than or equal to {1}.";

    public const string MaxCount = "The field {0} must contain at most {1} elements.";

    public const string ElementRequired = "The field {0} must contain at least one element and no empty elements.";

    public const string DuplicateKey = "The field {0} must not contain duplicate {1}.";
}
