namespace Template.ApiServer.Services;

using Microsoft.Extensions.DependencyInjection;

using Smart.Data;
using Smart.Mock.Data;

using Template.ApiServer.Accessors;

public sealed class DataServiceTests
{
    [Fact]
    public async Task CountAsyncReturnsScalar()
    {
        // Arrange
        await using var con = new MockDbConnection();
        con.SetupCommand(static cmd => cmd.SetupResult(3));
        await using var provider = CreateProvider(con);
        var service = provider.GetRequiredService<DataService>();

        // Act
        var count = await service.CountAsync(null, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task UpdateAsyncWithoutAffectedRowsReturnsNotFound()
    {
        // Arrange
        await using var con = new MockDbConnection();
        con.SetupCommand(static cmd => cmd.SetupResult(0));
        await using var provider = CreateProvider(con);
        var service = provider.GetRequiredService<DataService>();

        // Act
        var result = await service.UpdateAsync(1, "name", 100);

        // Assert
        Assert.Equal(DataWriteStatus.NotFound, result);
    }

    private static ServiceProvider CreateProvider(MockDbConnection con)
    {
        var services = new ServiceCollection();
        services.AddSingleton<IDbProvider>(new DelegateDbProvider(() => con));
        services.AddSingleton<IDialect>(new DelegateDialect(static _ => false, static x => x));
        services.AddSingleton(TimeProvider.System);
        services.AddDataAccessors(typeof(DataAccessor).Assembly);
        services.AddSingleton<DataService>();
        return services.BuildServiceProvider();
    }
}
