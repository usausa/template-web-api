namespace Template.ApiServer.Host.Settings;

public sealed class AuthSetting
{
    [Required]
    [MinLength(32)]
    public string SecretKey { get; set; } = default!;

    [Required]
    public string Issuer { get; set; } = default!;

    [Required]
    public string Audience { get; set; } = default!;

    [Range(1, 1440)]
    public int ExpireMinutes { get; set; }

    [Required]
    public string ApiKey { get; set; } = default!;

    public List<UserEntry> Users { get; } = [];

    public sealed class UserEntry
    {
        public string Id { get; set; } = default!;

        public string Password { get; set; } = default!;

        public List<string> Roles { get; } = [];
    }
}
