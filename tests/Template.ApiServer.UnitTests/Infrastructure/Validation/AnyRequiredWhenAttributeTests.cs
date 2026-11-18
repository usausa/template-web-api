namespace Template.ApiServer.Infrastructure.Validation;

using BunnyTail.MemberAccessor;

[GenerateAccessor]
[AnyRequiredWhen(nameof(Search), nameof(Code), nameof(Name))]
public sealed class AnyRequiredWhenModel
{
    public bool Search { get; set; }

    public string? Code { get; set; }

    public string? Name { get; set; }
}

public sealed class AnyRequiredWhenAttributeTests
{
    [Fact]
    public void UnmetConditionAllowsEmpty()
    {
        // Arrange
        var model = new AnyRequiredWhenModel { Search = false };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void MetConditionRequiresAnyValue()
    {
        // Arrange
        var model = new AnyRequiredWhenModel { Search = true };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal("At least one of the fields Code, Name is required.", result.ErrorMessage);
        Assert.Collection(result.MemberNames, x => Assert.Equal(nameof(AnyRequiredWhenModel.Code), x), x => Assert.Equal(nameof(AnyRequiredWhenModel.Name), x));
    }

    [Fact]
    public void MetConditionWithValueIsValid()
    {
        // Arrange
        var model = new AnyRequiredWhenModel { Search = true, Name = "name" };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        Assert.Empty(results);
    }
}
