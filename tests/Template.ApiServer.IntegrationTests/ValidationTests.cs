namespace Template.ApiServer;

using Microsoft.AspNetCore.Http;

using Template.ApiServer.Host.Endpoints;

public sealed class ValidationTests : IClassFixture<TestApplicationFactory>
{
    private static readonly Uri ValidationUri = new("/api/v1/test/validation", UriKind.Relative);

    private static readonly string[] InvalidPropertyKeys = ["entries", "entries[2].Code", "name", "phone", "to"];

    private static readonly string[] RequiredWhenKeys = ["from", "to"];

    private static readonly string[] AnyRequiredKeys = ["mail", "phone"];

    private readonly TestApplicationFactory factory;

    public ValidationTests(TestApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task ValidRequestReturnsOk()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = CreateValidRequest();

        // Act
        var response = await client.PostAsJsonAsync(ValidationUri, request, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task InvalidPropertiesReturnErrors()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = CreateValidRequest();
        request.Name = "ﾔﾏﾀﾞ";
        request.Phone = "03-1234-ABCD";
        request.From = new DateOnly(2026, 2, 1);
        request.To = new DateOnly(2026, 1, 1);
        request.Entries = [new TestValidationEntry { Code = "A1" }, new TestValidationEntry { Code = "A1" }, new TestValidationEntry { Code = "b2" }];

        // Act
        var response = await client.PostAsJsonAsync(ValidationUri, request, TestContext.Current.CancellationToken);
        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal(InvalidPropertyKeys, problem.Errors.Keys.Order(StringComparer.Ordinal));
    }

    [Fact]
    public async Task RequiredWhenReturnsErrors()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = CreateValidRequest();
        request.From = null;
        request.To = null;

        // Act
        var response = await client.PostAsJsonAsync(ValidationUri, request, TestContext.Current.CancellationToken);
        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal(RequiredWhenKeys, problem.Errors.Keys.Order(StringComparer.Ordinal));
    }

    [Fact]
    public async Task AnyRequiredReturnsErrorsForEachProperty()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = CreateValidRequest();
        request.Phone = null;
        request.Mail = null;

        // Act
        var response = await client.PostAsJsonAsync(ValidationUri, request, TestContext.Current.CancellationToken);
        var problem = await response.Content.ReadFromJsonAsync<HttpValidationProblemDetails>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(problem);
        Assert.Equal(AnyRequiredKeys, problem.Errors.Keys.Order(StringComparer.Ordinal));
    }

    private static TestValidationRequest CreateValidRequest() =>
        new()
        {
            Name = "山田",
            Phone = "03-1234-5678",
            UsePeriod = true,
            From = new DateOnly(2026, 1, 1),
            To = new DateOnly(2026, 1, 31),
            Entries = [new TestValidationEntry { Code = "A1" }, new TestValidationEntry { Code = "B2" }]
        };
}
