using Microsoft.AspNetCore.Authentication;

namespace API.CustomToken
{
    public class AuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "MyAuthenticationScheme";
        public string TokenHeaderName { get; set; } = "Token";
    }
}
