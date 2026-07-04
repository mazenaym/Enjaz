using System.Security.Cryptography;
using System.Text;
using Enjaz.Identity.Application.Auth;

namespace Enjaz.Identity.Infrastructure.Auth;

public sealed class OtpHasher : IOtpHasher
{
    public string Hash(string phoneNumber, string purpose, string code)
    {
        var value = $"{phoneNumber}:{purpose}:{code}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }

    public bool Verify(string phoneNumber, string purpose, string code, string hash)
    {
        return string.Equals(Hash(phoneNumber, purpose, code), hash, StringComparison.OrdinalIgnoreCase);
    }
}
