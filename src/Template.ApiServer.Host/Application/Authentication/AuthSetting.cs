namespace Template.ApiServer.Host.Application.Authentication;

using BunnyTail.CommonCode;

// 起動時ログへ設定内容を出力するため、シークレットをマスクしたToStringを生成する
#pragma warning disable CA1002
[GenerateToString]
public sealed partial class AuthSetting
{
    [Required]
    [MinLength(32)]
    [ToStringFormat(MaskChar = '*')]
    public string SecretKey { get; set; } = default!;

    [Required]
    public string Issuer { get; set; } = default!;

    [Required]
    public string Audience { get; set; } = default!;

    [Range(1, 1440)]
    public int ExpireMinutes { get; set; }

    [Required]
    [ToStringFormat(MaskChar = '*')]
    public string ApiKey { get; set; } = default!;

    public List<LoginUserOption> Users { get; } = [];
}
#pragma warning restore CA1002
