namespace Pollux.API.Auth
{
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
    using Pollux.Movies;
    using global::Movies.Common.Constants.Strings;

    /// <summary>
    /// TokenAuthenticationOptions.
    /// </summary>
    /// <seealso cref="AuthenticationSchemeOptions" />
    public class TokenAuthenticationOptions : AuthenticationSchemeOptions
    {
    }

    /// <summary>
    /// TokenAuthenticationHandler.
    /// </summary>
    /// <seealso cref="TokenAuthenticationOptions" />
    public class TokenAuthenticationHandler : AuthenticationHandler<TokenAuthenticationOptions>
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<ApplicationLogger> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly string tokenIssuer;
        private readonly string signingKeyId;

        public TokenAuthenticationHandler(
            IOptionsMonitor<TokenAuthenticationOptions> options,
            ILogger<ApplicationLogger> logger,
            ILoggerFactory loggerFactory,
            ISystemClock clock,
            IConfiguration configuration,
            UrlEncoder encoder)
            : base(options, loggerFactory, encoder, clock)
        {
            this.configuration = configuration;
            this.logger = logger;
            this.tokenIssuer = this.configuration.GetSection("AppSettings")["TokenIssuer"];
            this.signingKeyId = this.configuration.GetSection("AppSettings")["SigningKeyId"];
        }

        /// <summary>
        /// Handles the authenticate asynchronous.
        /// </summary>
        /// <returns>AuthenticateResult.</returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                this.Request.Headers.TryGetValue("Authorization", out var bearerToken);
                this.logger.LogInformation($"token {bearerToken}");
                if (string.IsNullOrEmpty(bearerToken))
                {
                    this.logger.LogInformation($"Initial Authorization token empty {bearerToken}");
                    return AuthenticateResult.Fail(AuthConstants.NotAuthenticated);
                }

                var claims = new[] { new Claim("token", bearerToken) };
                var identity = new ClaimsIdentity(claims, nameof(TokenAuthenticationHandler));
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), this.Scheme.Name);

                if (!string.IsNullOrEmpty(bearerToken))
                {
                    if (bearerToken.ToString()
                        .Contains("Bearer ", StringComparison.OrdinalIgnoreCase))
                    {
                        bearerToken = bearerToken.ToString().Remove(0, 7);
                        var isValid = this.ValidateToken(bearerToken);
                        if (!isValid)
                        {
                            return AuthenticateResult.Fail(AuthConstants.NotAuthenticated);
                        }

                        this.logger.LogInformation("Token is valid");
                        return AuthenticateResult.Success(ticket);
                    }
                }

                return AuthenticateResult.Fail(AuthConstants.NotAuthenticated);
            }
            catch (Exception ex)
            {
                this.logger.LogInformation("Unexpcted error");
                this.logger.LogInformation(ex.Message);
                this.logger.LogInformation(ex.StackTrace);
                this.logger.LogInformation(ex?.InnerException?.Message);
                this.Response.StatusCode = StatusCodes.Status500InternalServerError;
                return AuthenticateResult.Fail(AuthConstants.NotAuthenticated);
            }
        }

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="token">The token.</param>
        /// <returns>true/false</returns>
        private bool ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadJwtToken(token);
            var securityTokenSigningkeyId = securityToken.Header["kid"];
            if (securityToken.Issuer.Equals(this.tokenIssuer) &&
                this.signingKeyId.Equals(securityTokenSigningkeyId))
            {
                var claims = securityToken.Claims.ToList();
                var userId = claims.First(p => p.Type.Equals("sub")).Value;
                var expiration = claims[1].Value;
                var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiration)).UtcDateTime;

                return string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token) || DateTime.UtcNow <= expirationDate;
            }

            return false;
        }
    }
}