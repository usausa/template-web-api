namespace Template.ApiServer.Host.Endpoints;

using Template.ApiServer.Host.Application;
using Template.ApiServer.Infrastructure.Storage;

//--------------------------------------------------------------------------------
// Models
//--------------------------------------------------------------------------------

public sealed class FileListResponse
{
    public IReadOnlyList<string> Entries { get; set; } = default!;
}

public sealed class FileUploadEntry
{
    public string Name { get; set; } = default!;

    public long Size { get; set; }

    public string Path { get; set; } = default!;
}

public sealed class FileUploadResponse
{
    public int Uploaded { get; set; }

    public IReadOnlyList<FileUploadEntry> Files { get; set; } = default!;
}

//--------------------------------------------------------------------------------
// Endpoints
//--------------------------------------------------------------------------------

public static class FileEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapFileEndpoints(this WebApplication app)
    {
        var group = app.MapApiGroup(ApiRoutes.Files)
            .RequireAuthorization()
            .AddEndpointFilter(static async (context, next) =>
            {
                try
                {
                    return await next(context);
                }
                catch (StorageException)
                {
                    return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, title: "Invalid path.");
                }
            });

        group.MapGet("/list/{**path}", HandleListAsync);
        group.MapGet("/download/{**path}", HandleDownloadAsync);
        group.MapPost("/upload/{**path}", HandleUploadAsync)
            .DisableAntiforgery()
            .WithRequestTimeout(TimeSpan.FromMinutes(10));
        group.MapDelete("/{**path}", HandleDeleteAsync).RequireAuthorization(Policies.Administrator);
    }

    //--------------------------------------------------------------------------------
    // List
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleListAsync(
        IStorage storage,
        string? path,
        CancellationToken cancellationToken)
    {
        path ??= string.Empty;

        if (!await storage.DirectoryExistsAsync(path, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        var entries = await storage.ListAsync(path, cancellationToken);
        return TypedResults.Ok(new FileListResponse { Entries = entries });
    }

    //--------------------------------------------------------------------------------
    // Download
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleDownloadAsync(
        IStorage storage,
        string path,
        CancellationToken cancellationToken)
    {
        if (!await storage.FileExistsAsync(path, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        var stream = await storage.ReadAsync(path, cancellationToken);
        return TypedResults.Stream(stream, "application/octet-stream", Path.GetFileName(path));
    }

    //--------------------------------------------------------------------------------
    // Upload
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleUploadAsync(
        HttpContext context,
        IStorage storage,
        string? path)
    {
        var form = await context.Request.ReadFormAsync(context.RequestAborted);
        if (form.Files.Count == 0)
        {
            return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, title: "No files uploaded.");
        }

        var uploaded = new List<FileUploadEntry>();
        foreach (var file in form.Files)
        {
            var fileName = Path.GetFileName(file.FileName);
            var targetPath = String.IsNullOrEmpty(path) ? fileName : $"{path}/{fileName}";

            await using var stream = file.OpenReadStream();
            await storage.WriteAsync(targetPath, stream, context.RequestAborted);

            uploaded.Add(new FileUploadEntry { Name = fileName, Size = file.Length, Path = targetPath });
        }

        return TypedResults.Ok(new FileUploadResponse { Uploaded = uploaded.Count, Files = uploaded });
    }

    //--------------------------------------------------------------------------------
    // Delete
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleDeleteAsync(
        IStorage storage,
        string path,
        CancellationToken cancellationToken)
    {
        if (!await storage.FileExistsAsync(path, cancellationToken))
        {
            return TypedResults.NotFound();
        }

        await storage.DeleteAsync(path, cancellationToken);
        return TypedResults.NoContent();
    }
}
