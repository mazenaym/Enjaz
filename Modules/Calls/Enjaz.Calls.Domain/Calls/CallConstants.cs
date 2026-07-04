namespace Enjaz.Calls.Domain.Calls;

public static class CallProviders
{
    public const string Fake = "Fake";
    public const string Callera = "Callera";
    public static readonly IReadOnlySet<string> All = new HashSet<string> { Fake, Callera };
}

public static class CallStatuses
{
    public const string Created = "Created";
    public const string Ringing = "Ringing";
    public const string Connected = "Connected";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Missed = "Missed";
    public static readonly IReadOnlySet<string> All = new HashSet<string> { Created, Ringing, Connected, Completed, Failed, Missed };
}
