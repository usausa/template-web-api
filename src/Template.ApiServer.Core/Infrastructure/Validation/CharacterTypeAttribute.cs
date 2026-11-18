namespace Template.ApiServer.Infrastructure.Validation;

using System.Text.RegularExpressions;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed partial class CharacterTypeAttribute : ValidationAttribute
{
    private const string NoHalfWidthKanaPattern = @"[^\uFF61-\uFF9F]*";

    private const string FullWidthPattern = @"[^\u0000-\u007F\uFF61-\uFF9F]*";

    private const string UpperAlphanumericPattern = "[A-Z0-9]*";

    private const string UpperAlphanumericHyphenPattern = "[A-Z0-9-]*";

    private const string AlphanumericPattern = "[a-zA-Z0-9]*";

    private const string AlphanumericHyphenPattern = "[a-zA-Z0-9-]*";

    private const string NumericPattern = "[0-9]*";

    private const string NumericHyphenPattern = "[0-9-]*";

    private readonly Regex regex;

    public CharacterType CharacterType { get; }

    public string Pattern { get; }

    public CharacterTypeAttribute(CharacterType characterType)
        : base(() => ResolveMessage(characterType))
    {
        CharacterType = characterType;
        (Pattern, regex) = characterType switch
        {
            CharacterType.NoHalfWidthKana => (NoHalfWidthKanaPattern, NoHalfWidthKanaRegex()),
            CharacterType.FullWidth => (FullWidthPattern, FullWidthRegex()),
            CharacterType.UpperAlphanumeric => (UpperAlphanumericPattern, UpperAlphanumericRegex()),
            CharacterType.UpperAlphanumericHyphen => (UpperAlphanumericHyphenPattern, UpperAlphanumericHyphenRegex()),
            CharacterType.Alphanumeric => (AlphanumericPattern, AlphanumericRegex()),
            CharacterType.AlphanumericHyphen => (AlphanumericHyphenPattern, AlphanumericHyphenRegex()),
            CharacterType.Numeric => (NumericPattern, NumericRegex()),
            CharacterType.NumericHyphen => (NumericHyphenPattern, NumericHyphenRegex()),
            _ => throw new ArgumentOutOfRangeException(nameof(characterType))
        };
    }

    public override bool IsValid(object? value) =>
        value switch
        {
            string str => IsMatch(str),
            IEnumerable<string?> values => values.All(IsMatch),
            _ => true
        };

    private bool IsMatch(string? value)
    {
        if (String.IsNullOrEmpty(value))
        {
            return true;
        }

        var match = regex.Match(value);
        return match.Success && (match.Index == 0) && (match.Length == value.Length);
    }

    private static string ResolveMessage(CharacterType characterType) =>
        characterType switch
        {
            CharacterType.NoHalfWidthKana => ValidationMessages.NoHalfWidthKana,
            CharacterType.FullWidth => ValidationMessages.FullWidth,
            CharacterType.UpperAlphanumeric => ValidationMessages.UpperAlphanumeric,
            CharacterType.UpperAlphanumericHyphen => ValidationMessages.UpperAlphanumericHyphen,
            CharacterType.Alphanumeric => ValidationMessages.Alphanumeric,
            CharacterType.AlphanumericHyphen => ValidationMessages.AlphanumericHyphen,
            CharacterType.Numeric => ValidationMessages.Numeric,
            CharacterType.NumericHyphen => ValidationMessages.NumericHyphen,
            _ => throw new ArgumentOutOfRangeException(nameof(characterType))
        };

    [GeneratedRegex(NoHalfWidthKanaPattern)]
    private static partial Regex NoHalfWidthKanaRegex();

    [GeneratedRegex(FullWidthPattern)]
    private static partial Regex FullWidthRegex();

    [GeneratedRegex(UpperAlphanumericPattern)]
    private static partial Regex UpperAlphanumericRegex();

    [GeneratedRegex(UpperAlphanumericHyphenPattern)]
    private static partial Regex UpperAlphanumericHyphenRegex();

    [GeneratedRegex(AlphanumericPattern)]
    private static partial Regex AlphanumericRegex();

    [GeneratedRegex(AlphanumericHyphenPattern)]
    private static partial Regex AlphanumericHyphenRegex();

    [GeneratedRegex(NumericPattern)]
    private static partial Regex NumericRegex();

    [GeneratedRegex(NumericHyphenPattern)]
    private static partial Regex NumericHyphenRegex();
}
