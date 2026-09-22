namespace Template.ApiServer.Host.Endpoints;

using Smart.Mapper;

using Template.ApiServer.Host.Application;
using Template.ApiServer.Host.Infrastructure.Filters;
using Template.ApiServer.Host.Infrastructure.Http;
using Template.ApiServer.Host.Models.Data;

public static partial class DataEndpoints
{
    //--------------------------------------------------------------------------------
    // Mapping
    //--------------------------------------------------------------------------------

    public static void MapDataEndpoints(this WebApplication app)
    {
        var group = app.MapApiGroup(ApiRoutes.Data)
            .RequireAuthorization()
            .AddEndpointFilter<CredentialEndpointFilter>();

        group.MapGet("/", HandleListAsync);
        group.MapGet("/{id:long}", HandleGetAsync);
        group.MapPost("/", HandleCreateAsync);
        group.MapPut("/{id:long}", HandleUpdateAsync);
        group.MapDelete("/{id:long}", HandleDeleteAsync).RequireAuthorization(Policies.Administrator);
    }

    //--------------------------------------------------------------------------------
    // Handler
    //--------------------------------------------------------------------------------

    [Mapper]
    private static partial DataResponse ToResponse(DataEntity entity);

    private static async ValueTask<IResult> HandleListAsync(
        DataService dataService,
        string? name,
        string? sort,
        CancellationToken cancellationToken,
        bool desc = false,
        [Range(0, Int32.MaxValue)] int page = 0,
        [Range(1, 100)] int size = 20)
    {
        var result = await dataService.QueryPageAsync(name, sort, desc, page, size, cancellationToken);
        return TypedResults.Ok(new DataListResponse(
            result.Total,
            result.Page,
            result.Size,
            result.Items.Select(ToResponse).ToList()));
    }

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
        return TypedResults.Ok(ToResponse(entity));
    }

    private static async ValueTask<IResult> HandleCreateAsync(
        DataService dataService,
        HttpRequest httpRequest,
        DataCreateRequest request)
    {
        var id = await dataService.InsertAsync(request.Name, request.Value);
        return id.HasValue
            ? TypedResults.Created($"{httpRequest.Path}/{id.Value}", new DataCreateResponse(id.Value))
            : TypedResults.Problem(statusCode: StatusCodes.Status409Conflict, title: "Duplicate name.");
    }

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

    private static async ValueTask<IResult> HandleDeleteAsync(
        DataService dataService,
        long id)
    {
        var deleted = await dataService.DeleteAsync(id);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
