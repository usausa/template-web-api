namespace Template.ApiServer.Host.Settings;

public sealed class LogSetting
{
    public bool HttpLog { get; set; }

    // 開発用。要求と応答のボディ(JSON / XML)をHTTPログに含める
    public bool HttpDump { get; set; }

    [Range(1, 1_048_576)]
    public int HttpDumpLimit { get; set; }

    [Required]
    [ValidateObjectMembers]
    public W3CLogSetting W3CLog { get; set; } = default!;
}
