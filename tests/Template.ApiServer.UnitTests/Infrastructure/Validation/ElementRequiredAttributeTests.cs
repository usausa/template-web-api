namespace Template.ApiServer.Infrastructure.Validation;

public sealed class ElementRequiredAttributeTests
{
    [Fact]
    public void ElementsWithValuesAreValid()
    {
        // Arrange
        var attribute = new ElementRequiredAttribute();

        // Act
        var valid = attribute.IsValid(new List<string?> { "a", "b" });

        // Assert
        Assert.True(valid);
    }

    [Fact]
    public void NullAndEmptyAreInvalid()
    {
        // Arrange
        var attribute = new ElementRequiredAttribute();

        // Act
        var nullValue = attribute.IsValid(null);
        var empty = attribute.IsValid(Array.Empty<string>());

        // Assert
        Assert.False(nullValue);
        Assert.False(empty);
    }

    [Fact]
    public void EmptyElementIsInvalid()
    {
        // Arrange
        var attribute = new ElementRequiredAttribute();

        // Act
        var nullElement = attribute.IsValid(new List<string?> { "a", null });
        var whiteSpaceElement = attribute.IsValid(new List<string?> { "a", " " });

        // Assert
        Assert.False(nullElement);
        Assert.False(whiteSpaceElement);
    }

    [Fact]
    public void AllowEmptyStringsAcceptsWhiteSpaceElement()
    {
        // Arrange
        var attribute = new ElementRequiredAttribute { AllowEmptyStrings = true };

        // Act
        var valid = attribute.IsValid(new List<string?> { "a", " " });

        // Assert
        Assert.True(valid);
    }

    [Fact]
    public void FormatErrorMessageUsesName()
    {
        // Act
        var message = new ElementRequiredAttribute().FormatErrorMessage("Items");

        // Assert
        Assert.Equal("The field Items must contain at least one element and no empty elements.", message);
    }
}
