namespace Template.ApiServer.Infrastructure.Validation;

using BunnyTail.MemberAccessor;

[GenerateAccessor]
public sealed class RequiredWhenModel
{
    public bool Required { get; set; }

    [RequiredWhen(nameof(Required))]
    public string? Name { get; set; }

    [RequiredWhen(nameof(Required), AllowEmptyStrings = true)]
    public string? Note { get; set; }
}

public sealed class RequiredWhenAttributeTests
{
    [Fact]
    public void UnmetConditionAllowsNull()
    {
        // Arrange
        var model = new RequiredWhenModel { Required = false };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        Assert.Empty(results);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void MetConditionRequiresValue(string? value)
    {
        // Arrange
        var model = new RequiredWhenModel { Required = true, Name = value, Note = "note" };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        var result = Assert.Single(results);
        Assert.Equal("The Name field is required.", result.ErrorMessage);
        Assert.Equal(nameof(RequiredWhenModel.Name), Assert.Single(result.MemberNames));
    }

    [Fact]
    public void AllowEmptyStringsAcceptsEmpty()
    {
        // Arrange
        var model = new RequiredWhenModel { Required = true, Name = "name", Note = string.Empty };

        // Act
        var results = ValidationTestHelper.Validate(model);

        // Assert
        Assert.Empty(results);
    }
}
