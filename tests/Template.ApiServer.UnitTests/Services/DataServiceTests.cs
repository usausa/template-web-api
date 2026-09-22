namespace Template.ApiServer.Services;

using Microsoft.Extensions.DependencyInjection;

using Smart.Data;
using Smart.Mock.Data;

using Template.ApiServer.Accessors;
using Template.ApiServer.Models.Entity;

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
    public async Task UpdateAsyncReturnsNewVersion()
    {
        // Arrange
        await using var con = new MockDbConnection();
        con.SetupCommand(static cmd => cmd.SetupResult(2));
        await using var provider = CreateProvider(con);
        var service = provider.GetRequiredService<DataService>();

        // Act
        var result = await service.UpdateAsync(1, "name", 100, 1);

        // Assert
        Assert.Equal(DataWriteStatus.Success, result.Status);
        Assert.Equal(2, result.Version);
    }

    [Fact]
    public async Task UpdateAsyncWithoutAffectedRowsReturnsNotFound()
    {
        // Arrange
        await using var con = new MockDbConnection();
        con.SetupCommand(static cmd => cmd.SetupResult(DBNull.Value));
        await using var provider = CreateProvider(con);
        var service = provider.GetRequiredService<DataService>();

        // Act
        var result = await service.UpdateAsync(1, "name", 100);

        // Assert
        Assert.Equal(DataWriteStatus.NotFound, result.Status);
    }

    [Fact]
    public async Task UpdateAsyncWithStaleVersionReturnsVersionMismatch()
    {
        // Arrange
        await using var con = new MockDbConnection();
        con.SetupCommand(static cmd => cmd.SetupResult(DBNull.Value));
        con.SetupCommand(static cmd => cmd.SetupResult(MockHelper.CreateReader([new DataEntity { Id = 1, Name = "name", Value = 100, Version = 2 }])));
        await using var provider = CreateProvider(con);
        var service = provider.GetRequiredService<DataService>();

        // Act
        var result = await service.UpdateAsync(1, "name", 100, 1);

        // Assert
        Assert.Equal(DataWriteStatus.VersionMismatch, result.Status);
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
