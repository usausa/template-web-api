namespace Template.ApiServer.Accessors;

using Template.ApiServer.Models.Entity;

[DataAccessor]
public sealed partial class DataAccessor
{
    [ExecuteScalar]
    public partial ValueTask<int> CountAsync(string? name, CancellationToken cancellationToken);

    [Query]
    public partial ValueTask<List<DataEntity>> QueryPageAsync(string? name, DataSort sort, bool desc, int limit, int offset, CancellationToken cancellationToken);

    [QueryFirst]
    public partial ValueTask<DataEntity?> QueryAsync(long id);

    [ExecuteScalar]
    public partial ValueTask<long> InsertAsync(string name, int value, DateTime createdAt);

    [ExecuteScalar]
    public partial ValueTask<int?> UpdateAsync(long id, string name, int value, int? version);

    [Execute]
    public partial ValueTask<int> DeleteAsync(long id);
}
