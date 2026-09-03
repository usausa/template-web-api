namespace Template.ApiServer.Infrastructure.Storage;

public sealed class FileStorageTests : IDisposable
{
    private readonly string root = Path.Combine(Path.GetTempPath(), $"storage-test-{Guid.NewGuid():N}");

    private readonly FileStorage storage;

    public FileStorageTests()
    {
        Directory.CreateDirectory(root);
        storage = new FileStorage(new FileStorageOptions { Root = root });
    }

    public void Dispose()
    {
        try
        {
            Directory.Delete(root, true);
        }
        catch (IOException)
        {
            // Ignore
        }
    }

    [Fact]
    public async Task WriteAndReadRoundtrip()
    {
        // Arrange
        var content = "test content"u8.ToArray();

        // Act
        using (var input = new MemoryStream(content))
        {
            await storage.WriteAsync("test.txt", input, TestContext.Current.CancellationToken);
        }

        // Assert
        Assert.True(await storage.FileExistsAsync("test.txt", TestContext.Current.CancellationToken));
        await using var output = await storage.ReadAsync("test.txt", TestContext.Current.CancellationToken);
        using var buffer = new MemoryStream();
        await output.CopyToAsync(buffer, TestContext.Current.CancellationToken);
        Assert.Equal(content, buffer.ToArray());
    }

    [Fact]
    public async Task ListReturnsEntries()
    {
        // Arrange
        using (var input = new MemoryStream([0x01]))
        {
            await storage.WriteAsync("a.txt", input, TestContext.Current.CancellationToken);
        }

        // Act
        var entries = await storage.ListAsync(string.Empty, TestContext.Current.CancellationToken);

        // Assert
        Assert.Contains("a.txt", entries);
    }

    [Fact]
    public async Task TraversalPathThrows()
    {
        // Arrange & Act & Assert
        await Assert.ThrowsAsync<StorageException>(() => storage.ReadAsync("../evil.txt", TestContext.Current.CancellationToken).AsTask());
    }
}
