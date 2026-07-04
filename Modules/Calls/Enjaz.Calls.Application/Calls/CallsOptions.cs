namespace Enjaz.Calls.Application.Calls;

public sealed class CallsOptions
{
    public string Provider { get; set; } = "Fake";
    public CalleraOptions Callera { get; set; } = new();
}

public sealed class CalleraOptions
{
    public string? BaseUrl { get; set; }
    public string? ApiKey { get; set; }
    public string? WebhookSecret { get; set; }
}
