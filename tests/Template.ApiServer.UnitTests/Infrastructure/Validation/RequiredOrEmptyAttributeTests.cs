namespace Template.ApiServer.Infrastructure.Validation;

using BunnyTail.MemberAccessor;

[GenerateAccessor]
public sealed class RequiredOrEmptyModel
{
    public bool? UseLimit { get; set; }

    [RequiredOrEmpty(nameof(UseLimit))]
    public int? Limit { get; set; }
}

[GenerateAccessor]
public sealed class RequiredOrEmptyMessageModel
{
    public bool UseLimit { get; set; }

    [RequiredOrEmpty(nameof(UseLimit), EmptyErrorMessage = "{0} is not allowed.")]
    public int? Limit { get; set; }
}

public sealed class RequiredOrEmptyAttributeTests
{
    [Theory]
    [InlineData(true, 10)]
    [InlineData(false, null)]
    [InlineData(null, 10)]
    [InlineData(null, null)]
    public void IsValidMatchesCondition(bool? useLimit, int? limit)
    {
        // Arrange
        var model = new RequiredOrEmptyModel { UseLimit = useLimit, Limit = limit };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void MetConditionRequiresValue()
    {
        // Arrange
        var model = new RequiredOrEmptyModel { UseLimit = true, Limit = null };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal("The Limit field is required.", result.ErrorMessage);
        Assert.Equal(nameof(RequiredOrEmptyModel.Limit), Assert.Single(result.MemberNames));
    }

    [Fact]
    public void UnmetConditionRequiresEmpty()
    {
        // Arrange
        var model = new RequiredOrEmptyModel { UseLimit = false, Limit = 10 };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal("The Limit field must be empty.", result.ErrorMessage);
    }

    [Fact]
    public void EmptyErrorMessageCanBeChanged()
    {
        // Arrange
        var model = new RequiredOrEmptyMessageModel { UseLimit = false, Limit = 10 };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal("Limit is not allowed.", result.ErrorMessage);
    }
}
