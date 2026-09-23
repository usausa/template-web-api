namespace Template.ApiServer.Services;

using Template.ApiServer.Accessors;
using Template.ApiServer.Models;
using Template.ApiServer.Models.Entity;

public sealed class DataService
{
    private readonly IDialect dialect;

    private readonly DataAccessor dataAccessor;

    private readonly ServiceContextProvider contextProvider;

    public DataService(
        IDialect dialect,
        DataAccessor dataAccessor,
        ServiceContextProvider contextProvider)
    {
        this.dialect = dialect;
        this.dataAccessor = dataAccessor;
        this.contextProvider = contextProvider;
    }

    public ValueTask<int> CountAsync(string? name, CancellationToken cancellationToken = default) =>
        dataAccessor.CountAsync(dialect.Match(name), cancellationToken);

    public async ValueTask<PagedResult<DataEntity>> QueryPageAsync(string? name, DataSort sort, bool desc, int page, int size, CancellationToken cancellationToken = default)
    {
        var pattern = dialect.Match(name);
        var total = await dataAccessor.CountAsync(pattern, cancellationToken);
        var items = await dataAccessor.QueryPageAsync(pattern, sort, desc, size, page * size, cancellationToken);
        return new PagedResult<DataEntity>(total, page, size, items);
    }

    public ValueTask<DataEntity?> QueryAsync(long id) =>
        dataAccessor.QueryAsync(id);

    public async ValueTask<DataWriteStatus> InsertAsync(DataEntity entity)
    {
        var context = contextProvider.Current;

        try
        {
            entity.CreatedAt = context.Now.DateTime;
            entity.Version = 1;
            entity.Id = await dataAccessor.InsertAsync(entity.Name, entity.Value, entity.CreatedAt);
            return DataWriteStatus.Success;
        }
        catch (DbException ex) when (dialect.IsDuplicate(ex))
        {
            return DataWriteStatus.Duplicate;
        }
    }

    // version を指定した更新は一致した行だけ書き換える(楽観的同時実行制御)。null なら無条件
    public async ValueTask<DataUpdateResult> UpdateAsync(long id, string name, int value, int? version = null)
    {
        try
        {
            var updated = await dataAccessor.UpdateAsync(id, name, value, version);
            if (updated.HasValue)
            {
                return new DataUpdateResult(DataWriteStatus.Success, updated.Value);
            }

            // 行が無いのか版が違うのかを分ける
            var exists = version.HasValue && await dataAccessor.QueryAsync(id) is not null;
            return new DataUpdateResult(exists ? DataWriteStatus.VersionMismatch : DataWriteStatus.NotFound, 0);
        }
        catch (DbException ex) when (dialect.IsDuplicate(ex))
        {
            return new DataUpdateResult(DataWriteStatus.Duplicate, 0);
        }
    }

    public async ValueTask<DataWriteStatus> DeleteAsync(long id)
    {
        if (await dataAccessor.DeleteAsync(id) > 0)
        {
            return DataWriteStatus.Success;
        }

        return DataWriteStatus.NotFound;
    }
}
