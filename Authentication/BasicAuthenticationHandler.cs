using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using robot_controller_api.Contexts;
using System.Text.Encodings.Web;
using robot_controller_api.Models;
using System.Security.Claims;

namespace robot_controller_api.Authentication
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public BasicAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            base.Response.Headers.Add("WWW-Authenticate", @"Basic realm=""Access to the robot controller.""");
            var authHeader = base.Request.Headers["Authorization"].ToString();

            if (authHeader != "")
            {
                string[] base64_basic = authHeader.ToString().Split(" ");
                var base64 = Convert.FromBase64String(base64_basic[1]);
                string decoded = System.Text.Encoding.UTF8.GetString(base64);
                string[] decoded_split = decoded.ToString().Split(":");
                string email = decoded_split[0];
                string password = decoded_split[1];

                RobotContext robotContext = new RobotContext();

                try
                {
                    var user = robotContext.Users.Where(user => user.Email == email).First();

                    var hasher = new PasswordHasher<User>();
                    var pwVerificationResult = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

                    if (pwVerificationResult == PasswordVerificationResult.Success)
                    {
                        var claims = new[]
                        {
                             new Claim("name", $"{user.FirstName} {user.LastName}"),
                             new Claim(ClaimTypes.Role, user.Role ?? String.Empty),
                             new Claim(ClaimTypes.Email, user.Email)
                        };

                        var identity = new ClaimsIdentity(claims, "Basic");
                        var claimsPrincipal = new ClaimsPrincipal(identity);
                        var authTicket = new AuthenticationTicket(claimsPrincipal, Scheme.Name);
                        return Task.FromResult(AuthenticateResult.Success(authTicket));
                    }
                }
                catch(InvalidOperationException)
                {
                    Response.StatusCode = 401;
                    return Task.FromResult(AuthenticateResult.Fail($"User with {email} doesn't exist."));
                }


            }

            Response.StatusCode = 401;
            return Task.FromResult(AuthenticateResult.Fail(""));
        }
    }
}
