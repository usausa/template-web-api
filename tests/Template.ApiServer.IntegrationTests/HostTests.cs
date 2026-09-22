namespace Template.ApiServer;

public sealed class HostTests : IClassFixture<TestApplicationFactory>
{
    private readonly TestApplicationFactory factory;

    public HostTests(TestApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task HealthReturnsOk()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync(new Uri("/health", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task AliveReturnsOk()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync(new Uri("/alive", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ApiWithUnknownVersionReturnsNotFound()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync(new Uri("/api/v2/data", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ApiReportsSupportedVersions()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync(new Uri("/api/v1/test/time", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal("1.0", response.Headers.GetValues("api-supported-versions").Single());
    }

    [Fact]
    public async Task OpenApiDocumentContainsVersionedPath()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var document = await client.GetStringAsync(new Uri("/openapi/v1.json", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains("\"/api/v1/data\"", document, StringComparison.Ordinal);
        Assert.DoesNotContain("apiVersion", document, StringComparison.Ordinal);
    }

    [Fact]
    public async Task ApiWithoutAuthReturnsUnauthorized()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync(new Uri("/api/v1/data", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
