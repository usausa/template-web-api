namespace Template.ApiServer.Endpoints;

using System.ComponentModel.DataAnnotations;

using Template.ApiServer.Host.Endpoints;

public sealed class DataCreateRequestTests
{
    [Fact]
    public void ValidRequestPassesValidation()
    {
        // Arrange
        var request = new DataCreateRequest { Name = "name", Value = 100 };

        // Act
        var results = Validate(request);

        // Assert
        Assert.Empty(results);
    }

    [Fact]
    public void EmptyNameFailsValidation()
    {
        // Arrange
        var request = new DataCreateRequest { Name = string.Empty, Value = 100 };

        // Act
        var results = Validate(request);

        // Assert
        Assert.NotEmpty(results);
    }

    private static List<ValidationResult> Validate(object request)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(request, new ValidationContext(request), results, validateAllProperties: true);
        return results;
    }
}
