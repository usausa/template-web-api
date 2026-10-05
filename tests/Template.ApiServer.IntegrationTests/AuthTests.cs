namespace Template.ApiServer;

using System.Net.Http.Headers;
using System.Text;

using Template.ApiServer.Host.Endpoints;

public sealed class AuthTests : IClassFixture<TestApplicationFactory>
{
    // CA1861: Assertの比較対象は毎回同じ配列のため、呼び出しごとに生成しない
    private static readonly string[] SortedByName = ["SortItemA", "SortItemB", "SortItemC"];

    private static readonly int[] SortedByValueDescending = [30, 20, 10];

    private static readonly string[] InsertionOrder = ["SortItemB", "SortItemA", "SortItemC"];

    private readonly TestApplicationFactory factory;

    public AuthTests(TestApplicationFactory factory)
    {
        this.factory = factory;
    }

    [Fact]
    public async Task LoginReturnsToken()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(new Uri("/api/v1/auth/login", UriKind.Relative), new AuthLoginRequest { Id = "test", Password = "test" }, TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<AuthLoginResponse>(TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.NotNull(body);
        Assert.False(String.IsNullOrEmpty(body.Token));
    }

    [Fact]
    public async Task LoginWithWrongPasswordReturnsUnauthorized()
    {
        // Arrange
        var client = factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync(new Uri("/api/v1/auth/login", UriKind.Relative), new AuthLoginRequest { Id = "test", Password = "wrong" }, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DataApiWorksWithToken()
    {
        // Arrange
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync(new Uri("/api/v1/auth/login", UriKind.Relative), new AuthLoginRequest { Id = "test", Password = "test" }, TestContext.Current.CancellationToken);
        var token = (await login.Content.ReadFromJsonAsync<AuthLoginResponse>(TestContext.Current.CancellationToken))!.Token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var create = await client.PostAsJsonAsync(new Uri("/api/v1/data", UriKind.Relative), new DataCreateRequest { Name = "IntegrationItem", Value = 100 }, TestContext.Current.CancellationToken);
        var created = await create.Content.ReadFromJsonAsync<DataCreateResponse>(TestContext.Current.CancellationToken);
        var list = await client.GetFromJsonAsync<DataListResponse>(new Uri("/api/v1/data?name=IntegrationItem", UriKind.Relative), TestContext.Current.CancellationToken);

        // ロールポリシー(Administrator限定)の検証を兼ねる
        var delete = await client.DeleteAsync(new Uri($"/api/v1/data/{created!.Id}", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, create.StatusCode);
        Assert.NotNull(list);
        Assert.Single(list.Items);
        Assert.Equal("IntegrationItem", list.Items[0].Name);
        Assert.Equal(HttpStatusCode.NoContent, delete.StatusCode);
    }

    [Fact]
    public async Task ApiWorksWithApiKey()
    {
        // Arrange
        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "template-api-key");

        // Act
        var response = await client.GetAsync(new Uri("/api/v1/test/me", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreateWithInvalidBodyReturnsBadRequest()
    {
        // Arrange
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync(new Uri("/api/v1/auth/login", UriKind.Relative), new AuthLoginRequest { Id = "test", Password = "test" }, TestContext.Current.CancellationToken);
        var token = (await login.Content.ReadFromJsonAsync<AuthLoginResponse>(TestContext.Current.CancellationToken))!.Token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.PostAsJsonAsync(new Uri("/api/v1/data", UriKind.Relative), new DataCreateRequest { Name = string.Empty, Value = -1 }, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData("{")]
    [InlineData("""{"name":"JsonItem","value":"1"}""")]
    [InlineData("""{"name":"JsonItem","name":"JsonItem","value":1}""")]
    [InlineData("""{"name":"JsonItem","value":1,"ownerId":"other"}""")]
    public async Task CreateWithInvalidJsonReturnsBadRequest(string body)
    {
        // Arrange
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync(new Uri("/api/v1/auth/login", UriKind.Relative), new AuthLoginRequest { Id = "test", Password = "test" }, TestContext.Current.CancellationToken);
        var token = (await login.Content.ReadFromJsonAsync<AuthLoginResponse>(TestContext.Current.CancellationToken))!.Token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        using var content = new StringContent(body, Encoding.UTF8, "application/json");

        // Act
        var response = await client.PostAsync(new Uri("/api/v1/data", UriKind.Relative), content, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task DataApiRejectsStaleVersion()
    {
        // Arrange
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync(new Uri("/api/v1/auth/login", UriKind.Relative), new AuthLoginRequest { Id = "test", Password = "test" }, TestContext.Current.CancellationToken);
        var token = (await login.Content.ReadFromJsonAsync<AuthLoginResponse>(TestContext.Current.CancellationToken))!.Token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var create = await client.PostAsJsonAsync(new Uri("/api/v1/data", UriKind.Relative), new DataCreateRequest { Name = "VersionItem", Value = 1 }, TestContext.Current.CancellationToken);
        var id = (await create.Content.ReadFromJsonAsync<DataCreateResponse>(TestContext.Current.CancellationToken))!.Id;

        // Act
        var get = await client.GetAsync(new Uri($"/api/v1/data/{id}", UriKind.Relative), TestContext.Current.CancellationToken);
        var etag = get.Headers.ETag!.Tag;

        // 取得した版で更新 → 新しい版が返る
        using var matched = new HttpRequestMessage(HttpMethod.Put, new Uri($"/api/v1/data/{id}", UriKind.Relative));
        matched.Headers.IfMatch.ParseAdd(etag);
        matched.Content = JsonContent.Create(new DataUpdateRequest { Name = "VersionItem", Value = 2 });
        var updated = await client.SendAsync(matched, TestContext.Current.CancellationToken);

        // 古い版で更新 → 412
        using var stale = new HttpRequestMessage(HttpMethod.Put, new Uri($"/api/v1/data/{id}", UriKind.Relative));
        stale.Headers.IfMatch.ParseAdd(etag);
        stale.Content = JsonContent.Create(new DataUpdateRequest { Name = "VersionItem", Value = 3 });
        var rejected = await client.SendAsync(stale, TestContext.Current.CancellationToken);

        // If-Match なし → 無条件に更新
        var unconditional = await client.PutAsJsonAsync(new Uri($"/api/v1/data/{id}", UriKind.Relative), new DataUpdateRequest { Name = "VersionItem", Value = 4 }, TestContext.Current.CancellationToken);
        var latest = await client.GetFromJsonAsync<DataGetResponse>(new Uri($"/api/v1/data/{id}", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal("\"1\"", etag);
        Assert.Equal(HttpStatusCode.NoContent, updated.StatusCode);
        Assert.Equal("\"2\"", updated.Headers.ETag!.Tag);
        Assert.Equal(HttpStatusCode.PreconditionFailed, rejected.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, unconditional.StatusCode);
        Assert.NotNull(latest);
        Assert.Equal(3, latest.Version);
        Assert.Equal(4, latest.Value);
    }

    [Fact]
    public async Task DataApiSortsByRequestedColumn()
    {
        // Arrange
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync(new Uri("/api/v1/auth/login", UriKind.Relative), new AuthLoginRequest { Id = "test", Password = "test" }, TestContext.Current.CancellationToken);
        var token = (await login.Content.ReadFromJsonAsync<AuthLoginResponse>(TestContext.Current.CancellationToken))!.Token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 登録順とName順・Value順がいずれも異なるように積む
        await client.PostAsJsonAsync(new Uri("/api/v1/data", UriKind.Relative), new DataCreateRequest { Name = "SortItemB", Value = 20 }, TestContext.Current.CancellationToken);
        await client.PostAsJsonAsync(new Uri("/api/v1/data", UriKind.Relative), new DataCreateRequest { Name = "SortItemA", Value = 30 }, TestContext.Current.CancellationToken);
        await client.PostAsJsonAsync(new Uri("/api/v1/data", UriKind.Relative), new DataCreateRequest { Name = "SortItemC", Value = 10 }, TestContext.Current.CancellationToken);

        // Act
        var byName = await client.GetFromJsonAsync<DataListResponse>(new Uri("/api/v1/data?name=SortItem&sort=Name", UriKind.Relative), TestContext.Current.CancellationToken);
        var byValueDesc = await client.GetFromJsonAsync<DataListResponse>(new Uri("/api/v1/data?name=SortItem&sort=Value&desc=true", UriKind.Relative), TestContext.Current.CancellationToken);
        var unknownKey = await client.GetFromJsonAsync<DataListResponse>(new Uri("/api/v1/data?name=SortItem&sort=Unknown", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(byName);
        Assert.Equal(SortedByName, byName.Items.Select(static x => x.Name));
        Assert.NotNull(byValueDesc);
        Assert.Equal(SortedByValueDescending, byValueDesc.Items.Select(static x => x.Value));

        // 未知のキーはSQLのelse(Id順=登録順)へ落ちる
        Assert.NotNull(unknownKey);
        Assert.Equal(InsertionOrder, unknownKey.Items.Select(static x => x.Name));
    }
}
