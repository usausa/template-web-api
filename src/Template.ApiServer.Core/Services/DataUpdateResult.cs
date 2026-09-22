namespace Template.ApiServer.Services;

public readonly record struct DataUpdateResult(DataWriteStatus Status, int Version);
