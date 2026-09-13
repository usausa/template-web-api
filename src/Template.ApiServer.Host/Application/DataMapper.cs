namespace Template.ApiServer.Host.Application;

using Smart.Mapper;

using Template.ApiServer.Host.Models.Data;

internal static partial class DataMapper
{
    [Mapper]
    public static partial DataResponse ToResponse(this DataEntity entity);
}
