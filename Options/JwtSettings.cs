namespace MyAzureDemo.Options;

public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    public string Key { get; set; } = string.Empty;

    public string Issuer { get; set; } = "MyAzureDemo";

    public string Audience { get; set; } = "MyAzureDemo";

    public int ExpirationMinutes { get; set; } = 60;
}
