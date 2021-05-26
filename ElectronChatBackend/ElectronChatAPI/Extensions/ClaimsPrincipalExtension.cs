using System.Linq;

namespace ElectronChatAPI.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetUserName(this System.Security.Claims.ClaimsPrincipal principal)
        {
            return principal.Claims
                .Where(e => e.Type == System.Security.Claims.ClaimTypes.Name)
                .Select(e => e.Value).SingleOrDefault();
        }
    }
}