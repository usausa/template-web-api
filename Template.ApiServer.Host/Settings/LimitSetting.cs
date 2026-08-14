namespace Template.ApiServer.Host.Settings;

public sealed class LimitSetting
{
    [Required]
    public LimitEntry Global { get; set; } = default!;

    [Required]
    public LimitEntry Auth { get; set; } = default!;

    public sealed class LimitEntry
    {
        [Range(1, 3600)]
        public int Window { get; set; }

        [Range(1, Int32.MaxValue)]
        public int PermitLimit { get; set; }

        [Range(0, Int32.MaxValue)]
        public int QueueLimit { get; set; }
    }
}
