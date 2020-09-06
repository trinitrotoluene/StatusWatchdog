using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ServiceWatchdog.Api.Authentication
{
    public class InMemoryKeyAuthenticationHandler : AuthenticationHandler<InMemoryKeyAuthenticationHandlerOptions>
    {
        private readonly string _key;

        public InMemoryKeyAuthenticationHandler(
            IOptionsMonitor<InMemoryKeyAuthenticationHandlerOptions> optionsMonitor,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration config,
            IHostApplicationLifetime lifetime
        )
        : base(optionsMonitor, loggerFactory, encoder, clock)
        {
            try
            {
                var key = config.GetValue<string>("API_KEY");
                if (string.IsNullOrWhiteSpace(key)) throw new Exception();

                _key = key;
            }
            catch
            {
                Logger.LogCritical("Unable to read the API_KEY variable from the configuration. Set the WATCHDOG_API_KEY environment variable before starting the application or set the key in appsettings.json (not recommended) and restart.");
                lifetime.StopApplication();
            }
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.TryGetValue("Authorization", out var headerValues))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var key = headerValues.FirstOrDefault();

            if (!string.Equals(_key, key))
            {
                return Task.FromResult(AuthenticateResult.Fail("Access denied. Please provide a valid API key."));
            }

            var claims = new[] { new Claim(ClaimTypes.Name, "administrator"), new Claim(ClaimTypes.Role, "administrator") };
            var identity = new ClaimsIdentity(claims, Options.AuthenticationType);
            var principal = new GenericPrincipal(identity, new[] { "administrator" });
            var ticket = new AuthenticationTicket(principal, Options.Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
