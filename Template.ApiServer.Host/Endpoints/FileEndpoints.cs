namespace Template.ApiServer.Host.Endpoints;

using Template.ApiServer.Host.Application;
using Template.ApiServer.Host.Infrastructure.Filters;
using Template.ApiServer.Host.Models.File;
using Template.ApiServer.Infrastructure.Storage;

public static class FileEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapFileEndpoints(this WebApplication app)
    {
        var group = app.MapGroup(ApiRoutes.Files)
            .RequireAuthorization()
            .AddEndpointFilter<StorageExceptionFilter>();

        group.MapGet("/list/{**path}", HandleListAsync);
        group.MapGet("/download/{**path}", HandleDownloadAsync);
        group.MapPost("/upload/{**path}", HandleUploadAsync)
            .DisableAntiforgery()
            .WithRequestTimeout(TimeSpan.FromMinutes(10));
        group.MapDelete("/{**path}", HandleDeleteAsync).RequireAuthorization(Policies.Administrator);
    }

    //--------------------------------------------------------------------------------
    // Handler
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
        return TypedResults.Ok(new FileListResponse(entries));
    }

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

            uploaded.Add(new FileUploadEntry(fileName, file.Length, targetPath));
        }

        return TypedResults.Ok(new FileUploadResponse(uploaded.Count, uploaded));
    }

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
