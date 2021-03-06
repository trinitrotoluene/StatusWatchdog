using Microsoft.AspNetCore.Authentication;

namespace StatusWatchdog.Authentication
{
    public class InMemoryKeyAuthenticationHandlerOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "In-Memory Key Authentication";
        public string Scheme = DefaultScheme;
        public string AuthenticationType = DefaultScheme;
    }
}
