namespace Template.ApiServer.Host.Endpoints;

using Smart.Mapper;

using Template.ApiServer.Host.Application;
using Template.ApiServer.Host.Infrastructure.Http;

//--------------------------------------------------------------------------------
// Models
//--------------------------------------------------------------------------------

public sealed class DataListEntry
{
    public long Id { get; set; }

    public string Name { get; set; } = default!;

    public int Value { get; set; }

    public int Version { get; set; }

    public DateTime CreatedAt { get; set; }
}

public sealed class DataListResponse
{
    public int Total { get; set; }

    public int Page { get; set; }

    public int Size { get; set; }

    public IReadOnlyList<DataListEntry> Items { get; set; } = default!;
}

public sealed class DataResponse
{
    public long Id { get; set; }

    public string Name { get; set; } = default!;

    public int Value { get; set; }

    public int Version { get; set; }

    public DateTime CreatedAt { get; set; }
}

public sealed class DataCreateRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    [Range(0, 1_000_000)]
    public int Value { get; set; }
}

public sealed class DataCreateResponse
{
    public long Id { get; set; }
}

public sealed class DataUpdateRequest
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = default!;

    [Range(0, 1_000_000)]
    public int Value { get; set; }
}

//--------------------------------------------------------------------------------
// Mapper
//--------------------------------------------------------------------------------

public static partial class DataMapper
{
    [Mapper]
    public static partial DataListEntry ToListEntry(this DataEntity entity);

    [Mapper]
    public static partial DataResponse ToResponse(this DataEntity entity);
}

//--------------------------------------------------------------------------------
// Endpoints
//--------------------------------------------------------------------------------

public static class DataEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapDataEndpoints(this WebApplication app)
    {
        var group = app.MapApiGroup(ApiRoutes.Data)
            .RequireAuthorization();

        group.MapGet("/", HandleListAsync);
        group.MapGet("/{id:long}", HandleGetAsync);
        group.MapPost("/", HandleCreateAsync);
        group.MapPut("/{id:long}", HandleUpdateAsync);
        group.MapDelete("/{id:long}", HandleDeleteAsync).RequireAuthorization(Policies.Administrator);
    }

    //--------------------------------------------------------------------------------
    // List
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleListAsync(
        DataService dataService,
        string? name,
        string? sort,
        CancellationToken cancellationToken,
        bool desc = false,
        [Range(0, Int32.MaxValue)] int page = 0,
        [Range(1, 100)] int size = 20)
    {
        var result = await dataService.QueryPageAsync(name, RequestHelper.Parse(sort, DataSort.Id), desc, page, size, cancellationToken);
        return TypedResults.Ok(new DataListResponse
        {
            Total = result.Total,
            Page = result.Page,
            Size = result.Size,
            Items = result.Items.Select(static x => x.ToListEntry()).ToList()
        });
    }

    //--------------------------------------------------------------------------------
    // Get
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleGetAsync(
        DataService dataService,
        HttpResponse response,
        long id)
    {
        var entity = await dataService.QueryAsync(id);
        if (entity is null)
        {
            return TypedResults.NotFound();
        }

        response.Headers.ETag = EntityTag.From(entity.Version);
        return TypedResults.Ok(entity.ToResponse());
    }

    //--------------------------------------------------------------------------------
    // Create
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleCreateAsync(
        DataService dataService,
        HttpRequest httpRequest,
        DataCreateRequest request)
    {
        var entity = new DataEntity { Name = request.Name, Value = request.Value };
        return await dataService.InsertAsync(entity) == DataWriteStatus.Success
            ? TypedResults.Created($"{httpRequest.Path}/{entity.Id}", new DataCreateResponse { Id = entity.Id })
            : TypedResults.Problem(statusCode: StatusCodes.Status409Conflict, title: "Duplicate name.");
    }

    //--------------------------------------------------------------------------------
    // Update
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleUpdateAsync(
        DataService dataService,
        HttpResponse response,
        long id,
        DataUpdateRequest request,
        [FromHeader(Name = "If-Match")] string? ifMatch)
    {
        // If-Match の版と一致するときだけ更新する。無ければ無条件
        if (!EntityTag.TryParseIfMatch(ifMatch, out var version))
        {
            return TypedResults.Problem(statusCode: StatusCodes.Status412PreconditionFailed, title: "Version mismatch.");
        }

        var result = await dataService.UpdateAsync(id, request.Name, request.Value, version);
        switch (result.Status)
        {
            case DataWriteStatus.Success:
                response.Headers.ETag = EntityTag.From(result.Version);
                return TypedResults.NoContent();
            case DataWriteStatus.NotFound:
                return TypedResults.NotFound();
            case DataWriteStatus.VersionMismatch:
                return TypedResults.Problem(statusCode: StatusCodes.Status412PreconditionFailed, title: "Version mismatch.");
            default:
                return TypedResults.Problem(statusCode: StatusCodes.Status409Conflict, title: "Duplicate name.");
        }
    }

    //--------------------------------------------------------------------------------
    // Delete
    //--------------------------------------------------------------------------------

    private static async ValueTask<IResult> HandleDeleteAsync(
        DataService dataService,
        long id)
    {
        var result = await dataService.DeleteAsync(id);
        return result == DataWriteStatus.Success ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
