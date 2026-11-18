namespace Template.ApiServer.Infrastructure.Validation;

using BunnyTail.MemberAccessor;

[GenerateAccessor]
public sealed class CompareToModel
{
    public int Minimum { get; set; }

    [CompareTo(CompareToOperation.GreaterThanOrEqual, nameof(Minimum))]
    public int? Maximum { get; set; }
}

public sealed class CompareToWithoutAccessorModel
{
    public int Minimum { get; set; }

    [CompareTo(CompareToOperation.GreaterThanOrEqual, nameof(Minimum))]
    public int Maximum { get; set; }
}

public sealed class CompareToAttributeTests
{
    [Theory]
    [InlineData(CompareToOperation.LessThan, 1, 2, true)]
    [InlineData(CompareToOperation.LessThan, 2, 2, false)]
    [InlineData(CompareToOperation.LessThanOrEqual, 2, 2, true)]
    [InlineData(CompareToOperation.LessThanOrEqual, 3, 2, false)]
    [InlineData(CompareToOperation.GreaterThan, 3, 2, true)]
    [InlineData(CompareToOperation.GreaterThan, 2, 2, false)]
    [InlineData(CompareToOperation.GreaterThanOrEqual, 2, 2, true)]
    [InlineData(CompareToOperation.GreaterThanOrEqual, 1, 2, false)]
    public void IsValidComparesWithOtherProperty(CompareToOperation operation, int value, int other, bool expected)
    {
        // Arrange
        var attribute = new CompareToAttribute(operation, nameof(CompareToModel.Minimum));
        var model = new CompareToModel { Minimum = other, Maximum = value };

        // Act
        var result = ValidationTestHelper.ValidateProperty(attribute, model, nameof(CompareToModel.Maximum), value);

        // Assert
        Assert.Equal(expected, result is null);
    }

    [Fact]
    public void NullIsValid()
    {
        // Arrange
        var model = new CompareToModel { Minimum = 5, Maximum = null };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void ErrorUsesOtherPropertyName()
    {
        // Arrange
        var model = new CompareToModel { Minimum = 5, Maximum = 3 };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal("The field Maximum must be greater than or equal to Minimum.", result.ErrorMessage);
        Assert.Equal(nameof(CompareToModel.Maximum), Assert.Single(result.MemberNames));
    }

    [Fact]
    public void TypeWithoutAccessorThrows()
    {
        // Arrange
        var model = new CompareToWithoutAccessorModel { Minimum = 5, Maximum = 3 };

        // Act
        var exception = Record.Exception(() => ValidationTestHelper.Validate(model));

        // Assert
        Assert.IsType<InvalidOperationException>(exception);
    }
}
