using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Pollux.Movies.Auth
{
    /// <summary>
    /// TokenAuthenticationOptions.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions" />
    public class TokenAuthenticationOptions : AuthenticationSchemeOptions { }

    /// <summary>
    /// TokenAuthenticationHandler.
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Authentication.AuthenticationHandler&lt;Pollux.Movies.Auth.TokenAuthenticationOptions&gt;" />
    public class TokenAuthenticationHandler : AuthenticationHandler<TokenAuthenticationOptions>
    {
        private readonly IServiceProvider serviceProvider;
        private readonly IConfiguration configuration;

        public TokenAuthenticationHandler(IOptionsMonitor<TokenAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IServiceProvider serviceProvider, IConfiguration configuration)
       : base(options, logger, encoder, clock)
        {
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
        }

        /// <summary>
        /// Handles the authenticate asynchronous.
        /// </summary>
        /// <returns>AuthenticateResult.</returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                this.Request.Headers.TryGetValue("Authorization", out var token);
                var claims = new[] { new Claim("token", token) };
                var identity = new ClaimsIdentity(claims, nameof(TokenAuthenticationHandler));
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);

                if (!string.IsNullOrEmpty(token))
                {
                    if (token.ToString().Contains("Bearer", StringComparison.OrdinalIgnoreCase))
                    {
                        token = token.ToString().Remove(0, 7);
                        bool isValid = this.ValidateToken(token);
                        if (isValid)
                        {
                            return Task.FromResult(AuthenticateResult.Success(ticket));
                        }
                        else
                        {
                            return Task.FromResult(AuthenticateResult.Fail("Not Authenticated"));
                        }
                    }
                }

                this.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.FromResult(AuthenticateResult.Fail("Not Authenticated"));
            }
            catch (Exception ex)
            {
                this.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.FromResult(AuthenticateResult.Fail("Not Authenticated"));
            }
        }

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="token">The token.</param>
        private bool ValidateToken(string token)
        {
            try
            {
                var tokenIssuer = this.configuration.GetSection("AppSettings")["TokenIssuer"];
                var signingKeyId = this.configuration.GetSection("AppSettings")["SigningKeyId"];
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.ReadJwtToken(token);
                var securityTokenSigningkeyId = securityToken.Header["kid"];
                if (securityToken.Issuer.Equals(tokenIssuer) && signingKeyId.Equals(securityTokenSigningkeyId))
                {
                    var claims = securityToken.Claims.ToList();
                    var expiration = claims[1].Value;
                    var expirationDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiration));
                    if (DateTime.UtcNow > expirationDateTime)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}



