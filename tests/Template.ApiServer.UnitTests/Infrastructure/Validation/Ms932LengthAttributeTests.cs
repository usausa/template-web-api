namespace Template.ApiServer.Infrastructure.Validation;

public sealed class Ms932LengthAttributeTests
{
    [Theory]
    [InlineData("abcd", true)]
    [InlineData("abcde", false)]
    [InlineData("ｱｲｳｴ", true)]
    [InlineData("あい", true)]
    [InlineData("あいう", false)]
    public void IsValidCountsShiftJisBytes(string value, bool expected)
    {
        // Arrange
        var attribute = new Ms932LengthAttribute(4);

        // Act
        var valid = attribute.IsValid(value);

        // Assert
        Assert.Equal(expected, valid);
    }

    [Theory]
    [InlineData("a", false)]
    [InlineData("ab", true)]
    [InlineData("あい", true)]
    [InlineData("abcde", false)]
    public void IsValidChecksMinimumLength(string value, bool expected)
    {
        // Arrange
        var attribute = new Ms932LengthAttribute(2, 4);

        // Act
        var valid = attribute.IsValid(value);

        // Assert
        Assert.Equal(expected, valid);
    }

    [Fact]
    public void IsValidChecksEachElement()
    {
        // Arrange
        var attribute = new Ms932LengthAttribute(4);

        // Act
        var valid = attribute.IsValid(new List<string?> { "ab", null });
        var invalid = attribute.IsValid(new List<string?> { "ab", "あいう" });

        // Assert
        Assert.True(valid);
        Assert.False(invalid);
    }

    [Fact]
    public void NullIsValid()
    {
        // Arrange
        var attribute = new Ms932LengthAttribute(4);

        // Act
        var valid = attribute.IsValid(null);

        // Assert
        Assert.True(valid);
    }

    [Fact]
    public void FormatErrorMessageUsesLengths()
    {
        // Act
        var maximum = new Ms932LengthAttribute(4).FormatErrorMessage("Name");
        var range = new Ms932LengthAttribute(2, 4).FormatErrorMessage("Name");
        var fixedLength = new Ms932LengthAttribute(4, 4).FormatErrorMessage("Name");

        // Assert
        Assert.Equal("The field Name must be a string with a maximum length of 4 bytes in Shift_JIS.", maximum);
        Assert.Equal("The field Name must be a string with a minimum length of 2 bytes and a maximum length of 4 bytes in Shift_JIS.", range);
        Assert.Equal("The field Name must be a string with a length of 4 bytes in Shift_JIS.", fixedLength);
    }

    [Fact]
    public void InvalidLengthsThrow()
    {
        // Arrange
        var attribute = new Ms932LengthAttribute(5, 4);

        // Act
        var exception = Record.Exception(() => attribute.IsValid("a"));

        // Assert
        Assert.IsType<InvalidOperationException>(exception);
    }
}
