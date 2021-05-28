using System.Collections.Generic;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;

namespace ElectronChatAPI.Tests.BaseClasses.Auth
{
    public class TestAuthenticationOptions : AuthenticationSchemeOptions
    {
        public TestAuthenticationOptions()
        {
            this.Identity = new ClaimsIdentity(new List<Claim>(), "test");
        }

        public void SetIdentity(string name)
        {
            this.Identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", name));
        }

        public virtual ClaimsIdentity Identity { get; private set; }
    }
}
