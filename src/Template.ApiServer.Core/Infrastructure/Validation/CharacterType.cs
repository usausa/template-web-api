namespace Template.ApiServer.Infrastructure.Validation;

public enum CharacterType
{
    NoHalfWidthKana,
    FullWidth,
    UpperAlphanumeric,
    UpperAlphanumericHyphen,
    Alphanumeric,
    AlphanumericHyphen,
    Numeric,
    NumericHyphen
}
