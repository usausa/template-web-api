namespace Template.ApiServer.Host.Models.File;

public sealed record FileUploadEntry(string Name, long Size, string Path);

public sealed record FileUploadResponse(int Uploaded, IReadOnlyList<FileUploadEntry> Files);
