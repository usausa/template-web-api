namespace Template.ApiServer;

using System.Diagnostics.Metrics;
using System.Net.Http.Headers;

using Template.ApiServer.Host.Application.Telemetry;
using Template.ApiServer.Host.Models.Auth;

public sealed class MetricsTests : IClassFixture<TestApplicationFactory>
{
    private readonly TestApplicationFactory factory;

    private long count;

    public MetricsTests(TestApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task ApiRequestIsCounted()
    {
        // Arrange
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, l) =>
        {
            if ((instrument.Meter.Name == Source.Name) && (instrument.Name == "api.request.execution"))
            {
                l.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<long>((_, measurement, _, _) => Interlocked.Add(ref count, measurement));
        listener.Start();

        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync(new Uri("/api/auth/login", UriKind.Relative), new LoginRequest("test", "test"), TestContext.Current.CancellationToken);
        var token = (await login.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken))!.Token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.GetAsync(new Uri("/api/data", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.True(Interlocked.Read(ref count) >= 2);
    }
}
