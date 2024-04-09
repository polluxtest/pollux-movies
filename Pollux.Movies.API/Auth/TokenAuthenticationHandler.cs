using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

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
        private readonly ILogger<ApplicationLogger> logger;

        public TokenAuthenticationHandler(
            IOptionsMonitor<TokenAuthenticationOptions> options,
            ILogger<ApplicationLogger> logger,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock,
            IServiceProvider serviceProvider,
            IConfiguration configuration)
       : base(options, loggerFactory, encoder, clock)
        {
            this.logger = logger;
            this.serviceProvider = serviceProvider;
            this.configuration = configuration;
            this.logger = logger;
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
                this.logger.LogInformation($"token {token}");
                if (string.IsNullOrEmpty(token))
                {
                    this.logger.LogInformation($"Initial Authorization token empty {token}");
                    return Task.FromResult(AuthenticateResult.NoResult());
                }

                var claims = new[] { new Claim("token", token) };
                var identity = new ClaimsIdentity(claims, nameof(TokenAuthenticationHandler));
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);

                if (!string.IsNullOrEmpty(token))
                {
                    this.logger.LogInformation("token is not null and token decoded is the same ok.");
                    if (token.ToString().Contains("Bearer", StringComparison.OrdinalIgnoreCase))
                    {
                        token = token.ToString().Remove(0, 7);
                        bool isValid = this.ValidateToken(token);
                        if (isValid)
                        {
                            this.logger.LogInformation("Token is valid");
                            return Task.FromResult(AuthenticateResult.Success(ticket));
                        }
                        else
                        {
                            this.logger.LogInformation("Token is invalid");
                            return Task.FromResult(AuthenticateResult.Fail("Not Authenticated"));
                        }
                    }
                }

                this.logger.LogInformation("Authorization failed 401");
                this.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.FromResult(AuthenticateResult.Fail("Not Authenticated"));
            }
            catch (Exception ex)
            {
                this.logger.LogInformation("Authorization exception");
                this.logger.LogInformation(ex.Message);
                this.logger.LogInformation(ex.StackTrace);
                this.logger.LogInformation(ex?.InnerException?.Message);
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
                this.logger.LogInformation($"token issuer app settings {tokenIssuer}");
                var signingKeyId = this.configuration.GetSection("AppSettings")["SigningKeyId"];
                this.logger.LogInformation($"signing key app settings {signingKeyId}");

                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.ReadJwtToken(token);
                this.logger.LogInformation($"token decoded jwt issuer {securityToken.Issuer}");
                var securityTokenSigningkeyId = securityToken.Header["kid"];
                this.logger.LogInformation($"signing key request header {securityTokenSigningkeyId}");

                if (securityToken.Issuer.Equals(tokenIssuer))
                {
                    this.logger.LogInformation("Token issuer matched");
                    if (signingKeyId.Equals(securityTokenSigningkeyId))
                    {
                        this.logger.LogInformation("Signing key matched");
                        var claims = securityToken.Claims.ToList();
                        this.logger.LogInformation($"claims  {claims}");
                        this.logger.LogInformation("token issuer and token decoded is the same ok.");
                        var expiration = claims[1].Value;

                        this.logger.LogInformation($"expiration  {expiration}");

                        var expirationDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiration));
                        if (DateTime.UtcNow > expirationDateTime)
                        {
                            this.logger.LogInformation($"expired token");
                            return false;
                        }
                    }
                    else
                    {
                        this.logger.LogInformation("signing key NOT MATCHED");
                    }
                }
                else
                {
                    this.logger.LogInformation("token issuer NOT MATCHED");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                this.logger.LogError($"error exception  {ex.Message}");
                this.logger.LogError($"error exception  {ex.StackTrace}");
                this.logger.LogError($"error exception  {ex.InnerException}");

                return false;
            }
        }
    }
}



