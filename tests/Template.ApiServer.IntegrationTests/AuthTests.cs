namespace Template.ApiServer;

using System.Net.Http.Headers;

using Template.ApiServer.Host.Models.Auth;
using Template.ApiServer.Host.Models.Data;

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
        var response = await client.PostAsJsonAsync(new Uri("/api/auth/login", UriKind.Relative), new LoginRequest("test", "test"), TestContext.Current.CancellationToken);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken);

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
        var response = await client.PostAsJsonAsync(new Uri("/api/auth/login", UriKind.Relative), new LoginRequest("test", "wrong"), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task DataApiWorksWithToken()
    {
        // Arrange
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync(new Uri("/api/auth/login", UriKind.Relative), new LoginRequest("test", "test"), TestContext.Current.CancellationToken);
        var token = (await login.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken))!.Token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var create = await client.PostAsJsonAsync(new Uri("/api/data", UriKind.Relative), new DataCreateRequest("IntegrationItem", 100), TestContext.Current.CancellationToken);
        var created = await create.Content.ReadFromJsonAsync<DataCreateResponse>(TestContext.Current.CancellationToken);
        var list = await client.GetFromJsonAsync<DataListResponse>(new Uri("/api/data?name=IntegrationItem", UriKind.Relative), TestContext.Current.CancellationToken);

        // ロールポリシー(Administrator限定)の検証を兼ねる
        var delete = await client.DeleteAsync(new Uri($"/api/data/{created!.Id}", UriKind.Relative), TestContext.Current.CancellationToken);

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
        var response = await client.GetAsync(new Uri("/api/test/me", UriKind.Relative), TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task CreateWithInvalidBodyReturnsBadRequest()
    {
        // Arrange
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync(new Uri("/api/auth/login", UriKind.Relative), new LoginRequest("test", "test"), TestContext.Current.CancellationToken);
        var token = (await login.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken))!.Token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await client.PostAsJsonAsync(new Uri("/api/data", UriKind.Relative), new DataCreateRequest(string.Empty, -1), TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task DataApiSortsByRequestedColumn()
    {
        // Arrange
        var client = factory.CreateClient();
        var login = await client.PostAsJsonAsync(new Uri("/api/auth/login", UriKind.Relative), new LoginRequest("test", "test"), TestContext.Current.CancellationToken);
        var token = (await login.Content.ReadFromJsonAsync<LoginResponse>(TestContext.Current.CancellationToken))!.Token;
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // 登録順とName順・Value順がいずれも異なるように積む
        await client.PostAsJsonAsync(new Uri("/api/data", UriKind.Relative), new DataCreateRequest("SortItemB", 20), TestContext.Current.CancellationToken);
        await client.PostAsJsonAsync(new Uri("/api/data", UriKind.Relative), new DataCreateRequest("SortItemA", 30), TestContext.Current.CancellationToken);
        await client.PostAsJsonAsync(new Uri("/api/data", UriKind.Relative), new DataCreateRequest("SortItemC", 10), TestContext.Current.CancellationToken);

        // Act
        var byName = await client.GetFromJsonAsync<DataListResponse>(new Uri("/api/data?name=SortItem&sort=Name", UriKind.Relative), TestContext.Current.CancellationToken);
        var byValueDesc = await client.GetFromJsonAsync<DataListResponse>(new Uri("/api/data?name=SortItem&sort=Value&desc=true", UriKind.Relative), TestContext.Current.CancellationToken);
        var unknownKey = await client.GetFromJsonAsync<DataListResponse>(new Uri("/api/data?name=SortItem&sort=Unknown", UriKind.Relative), TestContext.Current.CancellationToken);

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
