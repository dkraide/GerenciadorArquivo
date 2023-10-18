using Azure.Core;
using Communication.Contexts;
using Communication.Models;
using Communication.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace API.CustomToken
{
    public class AuthHandler :
    AuthenticationHandler<AuthOptions>
    {
        private readonly SUser _userService;
        public AuthHandler
            (
            Context context,
            UserManager<MUser> userManager,
            IOptionsMonitor<AuthOptions> options,
            ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _userService = new SUser(context, userManager);
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers
                .ContainsKey(Options.TokenHeaderName))
            {
                return AuthenticateResult.Fail($"Missing header: {Options.TokenHeaderName}");
            }

            string token = Request
                .Headers[Options.TokenHeaderName]!;

            var user = _userService.GetUser(token);

            //usually, this is where you decrypt a token and/or lookup a database.
            if (user == null)
            {
                return AuthenticateResult
                    .Fail($"Invalid token.");
            }
            //Success! Add details here that identifies the user
            var claims = new List<Claim>()
        {
            new Claim("id", token)
        };

            var claimsIdentity = new ClaimsIdentity
                (claims, this.Scheme.Name);
            var claimsPrincipal = new ClaimsPrincipal
                (claimsIdentity);

            return AuthenticateResult.Success
                (new AuthenticationTicket(claimsPrincipal,
                this.Scheme.Name));
        }
    }
}