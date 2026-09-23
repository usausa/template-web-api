namespace Template.ApiServer.Host.Application.Authentication;

using BunnyTail.CommonCode;

// DefaultLoginProvider が照合する利用者
#pragma warning disable CA1002
[GenerateToString]
public sealed partial class LoginUserOption
{
    public string Id { get; set; } = default!;

    [ToStringFormat(MaskChar = '*')]
    public string Password { get; set; } = default!;

    public List<string> Roles { get; } = [];
}
#pragma warning restore CA1002
