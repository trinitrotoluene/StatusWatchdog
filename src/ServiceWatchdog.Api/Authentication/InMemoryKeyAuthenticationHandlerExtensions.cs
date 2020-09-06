using System;
using Microsoft.AspNetCore.Authentication;

namespace ServiceWatchdog.Api.Authentication
{
    public static class InMemoryKeyAuthenticationHandlerExtensions
    {
        public static AuthenticationBuilder UseInMemoryKey(this AuthenticationBuilder builder)
        {
            builder.AddScheme<InMemoryKeyAuthenticationHandlerOptions, InMemoryKeyAuthenticationHandler>(InMemoryKeyAuthenticationHandlerOptions.DefaultScheme, x => { });
            return builder;
        }

        public static AuthenticationBuilder UseInMemoryKey(this AuthenticationBuilder builder, Action<InMemoryKeyAuthenticationHandlerOptions> configureOptions)
        {
            builder.AddScheme<InMemoryKeyAuthenticationHandlerOptions, InMemoryKeyAuthenticationHandler>(InMemoryKeyAuthenticationHandlerOptions.DefaultScheme, configureOptions);
            return builder;
        }
    }
}
