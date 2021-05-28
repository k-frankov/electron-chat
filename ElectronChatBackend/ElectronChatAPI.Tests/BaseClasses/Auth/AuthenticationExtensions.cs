using System;

using Microsoft.AspNetCore.Authentication;

namespace ElectronChatAPI.Tests.BaseClasses.Auth
{
    public static class AuthenticationExtensions
    {
        public static AuthenticationBuilder AddTestAuth(this AuthenticationBuilder builder, Action<TestAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>("Test Scheme", "Test Auth", configureOptions);
        }
    }
}
