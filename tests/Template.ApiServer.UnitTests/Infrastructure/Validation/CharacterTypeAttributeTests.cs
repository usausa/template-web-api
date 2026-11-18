namespace Template.ApiServer.Infrastructure.Validation;

public sealed class CharacterTypeAttributeTests
{
    [Theory]
    [InlineData(CharacterType.NoHalfWidthKana, "あいうABC", true)]
    [InlineData(CharacterType.NoHalfWidthKana, "ｱｲｳ", false)]
    [InlineData(CharacterType.NoHalfWidthKana, "あ｡", false)]
    [InlineData(CharacterType.FullWidth, "あいうＡＢＣ", true)]
    [InlineData(CharacterType.FullWidth, "あいうA", false)]
    [InlineData(CharacterType.FullWidth, "あ い", false)]
    [InlineData(CharacterType.FullWidth, "あｱ", false)]
    [InlineData(CharacterType.UpperAlphanumeric, "ABC123", true)]
    [InlineData(CharacterType.UpperAlphanumeric, "ABc123", false)]
    [InlineData(CharacterType.UpperAlphanumeric, "AB-12", false)]
    [InlineData(CharacterType.UpperAlphanumericHyphen, "AB-12", true)]
    [InlineData(CharacterType.UpperAlphanumericHyphen, "ab-12", false)]
    [InlineData(CharacterType.Alphanumeric, "abcXYZ09", true)]
    [InlineData(CharacterType.Alphanumeric, "abc-09", false)]
    [InlineData(CharacterType.AlphanumericHyphen, "abc-XYZ-09", true)]
    [InlineData(CharacterType.AlphanumericHyphen, "abc_09", false)]
    [InlineData(CharacterType.Numeric, "0123", true)]
    [InlineData(CharacterType.Numeric, "12a", false)]
    [InlineData(CharacterType.Numeric, "１２", false)]
    [InlineData(CharacterType.NumericHyphen, "03-1234-5678", true)]
    [InlineData(CharacterType.NumericHyphen, "03 1234", false)]
    public void IsValidChecksCharacters(CharacterType characterType, string value, bool expected)
    {
        // Arrange
        var attribute = new CharacterTypeAttribute(characterType);

        // Act
        var valid = attribute.IsValid(value);

        // Assert
        Assert.Equal(expected, valid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void EmptyIsValid(string? value)
    {
        // Arrange
        var attribute = new CharacterTypeAttribute(CharacterType.Numeric);

        // Act
        var valid = attribute.IsValid(value);

        // Assert
        Assert.True(valid);
    }

    [Fact]
    public void TrailingNewLineIsInvalid()
    {
        // Arrange
        var attribute = new CharacterTypeAttribute(CharacterType.Numeric);

        // Act
        var valid = attribute.IsValid("123" + Environment.NewLine);

        // Assert
        Assert.False(valid);
    }

    [Fact]
    public void IsValidChecksEachElement()
    {
        // Arrange
        var attribute = new CharacterTypeAttribute(CharacterType.Numeric);

        // Act
        var valid = attribute.IsValid(new List<string?> { "12", null, string.Empty });
        var invalid = attribute.IsValid(new List<string?> { "12", "ab" });

        // Assert
        Assert.True(valid);
        Assert.False(invalid);
    }

    [Fact]
    public void FormatErrorMessageDependsOnCharacterType()
    {
        // Act
        var numeric = new CharacterTypeAttribute(CharacterType.Numeric).FormatErrorMessage("Code");
        var fullWidth = new CharacterTypeAttribute(CharacterType.FullWidth).FormatErrorMessage("Name");

        // Assert
        Assert.Equal("The field Code must contain only digits.", numeric);
        Assert.Equal("The field Name must contain only full-width characters.", fullWidth);
    }

    [Fact]
    public void PatternMatchesCharacterType()
    {
        // Act
        var pattern = new CharacterTypeAttribute(CharacterType.NumericHyphen).Pattern;

        // Assert
        Assert.Equal("[0-9-]*", pattern);
    }
}
