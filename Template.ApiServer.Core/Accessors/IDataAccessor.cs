namespace Template.ApiServer.Accessors;

using Template.ApiServer.Models.Entity;

[DataAccessor]
public interface IDataAccessor
{
    [Execute]
    void Create();

    [ExecuteScalar]
    ValueTask<int> CountAsync(string? name);

    [Query]
    ValueTask<List<DataEntity>> QueryPageAsync(string? name, int offset, int size);

    [QueryFirstOrDefault]
    ValueTask<DataEntity?> QueryAsync(long id);

    [ExecuteScalar]
    ValueTask<long> InsertAsync(string name, int value, DateTime createdAt);

    [Execute]
    ValueTask<int> UpdateAsync(long id, string name, int value);

    [Execute]
    ValueTask<int> DeleteAsync(long id);
}
