namespace Template.ApiServer.Host.Settings;

public sealed class LimitSetting
{
    [Required]
    [ValidateObjectMembers]
    public LimitEntry Global { get; set; } = default!;

    [Required]
    [ValidateObjectMembers]
    public LimitEntry Auth { get; set; } = default!;
}
