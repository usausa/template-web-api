namespace Template.ApiServer.Infrastructure.Validation;

public sealed class MaxCountAttributeTests
{
    [Theory]
    [InlineData(0, true)]
    [InlineData(2, true)]
    [InlineData(3, false)]
    public void IsValidChecksCount(int count, bool expected)
    {
        // Arrange
        var attribute = new MaxCountAttribute(2);

        // Act
        var array = attribute.IsValid(new int[count]);
        var list = attribute.IsValid(new List<int>(new int[count]));
        var enumerable = attribute.IsValid(Enumerable.Range(0, count));

        // Assert
        Assert.Equal(expected, array);
        Assert.Equal(expected, list);
        Assert.Equal(expected, enumerable);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("abc")]
    public void NonCollectionIsValid(string? value)
    {
        // Arrange
        var attribute = new MaxCountAttribute(2);

        // Act
        var valid = attribute.IsValid(value);

        // Assert
        Assert.True(valid);
    }

    [Fact]
    public void FormatErrorMessageUsesCount()
    {
        // Act
        var message = new MaxCountAttribute(2).FormatErrorMessage("Items");

        // Assert
        Assert.Equal("The field Items must contain at most 2 elements.", message);
    }

    [Fact]
    public void NegativeCountThrows()
    {
        // Arrange
        var attribute = new MaxCountAttribute(-1);

        // Act
        var exception = Record.Exception(() => attribute.IsValid(Array.Empty<int>()));

        // Assert
        Assert.IsType<InvalidOperationException>(exception);
    }
}
