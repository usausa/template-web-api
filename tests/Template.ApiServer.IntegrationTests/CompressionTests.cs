namespace Template.ApiServer;

using System.IO.Compression;
using System.Net.Http.Headers;
using System.Text.Json;

using Template.ApiServer.Host.Models.Auth;

public sealed class CompressionTests : IClassFixture<TestApplicationFactory>
{
    private readonly TestApplicationFactory factory;

    public CompressionTests(TestApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task ResponseIsCompressedWhenAccepted()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));

        // Act
        var response = await client.GetAsync(new Uri("/openapi/v1.json", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Contains("gzip", response.Content.Headers.ContentEncoding);
    }

    [Fact]
    public async Task CompressedRequestIsDecompressedWhenEnabled()
    {
        // Arrange (Compression:Request は既定 false)
        await using var enabled = factory.WithWebHostBuilder(static builder => builder.UseSetting("Compression:Request", "true"));
        var client = enabled.CreateClient();
        using var body = new MemoryStream();
        await using (var gzip = new GZipStream(body, CompressionMode.Compress, leaveOpen: true))
        {
            await JsonSerializer.SerializeAsync(gzip, new LoginRequest("test", "test"), JsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        }

        using var content = new ByteArrayContent(body.ToArray());
        content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        content.Headers.ContentEncoding.Add("gzip");

        // Act
        var response = await client.PostAsync(new Uri("/api/auth/login", UriKind.Relative), content, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(await response.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken));
    }
}
