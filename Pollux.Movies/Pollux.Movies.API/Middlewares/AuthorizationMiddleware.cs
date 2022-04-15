using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Pollux.Movies.Middlewares
{
    /// <summary>
    /// Middleware to verify the access token emited by identity server in Pollux Microservice.
    /// </summary>
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate nextDelegate;
        private readonly IConfiguration configuration;

        public AuthorizationMiddleware(RequestDelegate nextDelegate, IConfiguration configuration)
        {
            this.nextDelegate = nextDelegate;
            this.configuration = configuration;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                httpContext.Request.Headers.TryGetValue("Authorization", out var token);

                if (!string.IsNullOrEmpty(token))
                {
                    if (token.ToString().Contains("Bearer", StringComparison.OrdinalIgnoreCase))
                    {
                        token = token.ToString().Remove(0, 7);
                        this.ValidateToken(token);
                    }
                }

                await this.nextDelegate.Invoke(httpContext);
            }
            catch (Exception)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("Not Authenticated");
            }
        }

        /// <summary>
        /// Validates the token.
        /// </summary>
        /// <param name="token">The token.</param>
        private void ValidateToken(string token)
        {
            try
            {
                var tokenIssuer = this.configuration.GetSection("Pollux")["TokenIssuer"];
                var tokenHandler = new JwtSecurityTokenHandler();
                var securityToken = tokenHandler.ReadJwtToken(token);
                if (securityToken.Issuer.Equals(tokenIssuer))
                {
                    var claims = securityToken.Claims.ToList();
                    var expiration = claims[1].Value;
                    var expirationDateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expiration));
                    if (DateTime.UtcNow > expirationDateTime)
                    {
                        throw new Exception();
                    }
                }
                else
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}



