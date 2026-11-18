namespace Template.ApiServer.Infrastructure.Validation;

using BunnyTail.MemberAccessor;

[GenerateAccessor]
public sealed class CompareToWhenModel
{
    public bool UsePeriod { get; set; }

    public DateOnly? From { get; set; }

    [CompareToWhen(nameof(UsePeriod), CompareToOperation.GreaterThanOrEqual, nameof(From))]
    public DateOnly? To { get; set; }
}

public sealed class CompareToWhenAttributeTests
{
    [Fact]
    public void UnmetConditionSkipsComparison()
    {
        // Arrange
        var model = new CompareToWhenModel { UsePeriod = false, From = new DateOnly(2026, 2, 1), To = new DateOnly(2026, 1, 1) };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void MetConditionComparesWithOtherProperty()
    {
        // Arrange
        var model = new CompareToWhenModel { UsePeriod = true, From = new DateOnly(2026, 2, 1), To = new DateOnly(2026, 1, 1) };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal("The field To must be greater than or equal to From.", result.ErrorMessage);
        Assert.Equal(nameof(CompareToWhenModel.To), Assert.Single(result.MemberNames));
    }

    [Fact]
    public void MetConditionWithValidOrderIsValid()
    {
        // Arrange
        var model = new CompareToWhenModel { UsePeriod = true, From = new DateOnly(2026, 1, 1), To = new DateOnly(2026, 1, 1) };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        Assert.Empty(results);
    }
}
