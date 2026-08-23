namespace Template.ApiServer.Host.Mappers;

using Smart.Mapper;

using Template.ApiServer.Host.Models.Data;

internal static partial class DataMapper
{
    [Mapper]
    public static partial DataResponse ToResponse(DataEntity entity);
}
