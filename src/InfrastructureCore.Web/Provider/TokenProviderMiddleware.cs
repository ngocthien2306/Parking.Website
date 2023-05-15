using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using InfrastructureCore.Web.Services.IService;
using System.Globalization;

namespace InfrastructureCore.Web.Provider
{
    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly TokenProviderOptions _options;
        private readonly IUserManager _userManager;

        public TokenProviderMiddleware(
            RequestDelegate next,
            IOptions<TokenProviderOptions> options, IUserManager userManager)
        {
            _next = next;
            _options = options.Value;
            this._userManager = userManager;
        }

        public Task Invoke(HttpContext context)
        {
            
            string culture = CultureInfo.CurrentCulture.TextInfo.CultureName;
            if (!context.Request.Path.Equals("/" + culture + _options.Path, StringComparison.Ordinal))
            {
                return _next(context);
            }

            if (!context.Request.Method.Equals("POST")
               || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            return GenerateToken(context);
            //return null;
        }

        private async Task GenerateToken(HttpContext context)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];

            var identity = await GetIdentity(context, username, password);

            if (identity == null)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync("Invalid username or password.");
                return;
            }

            var now = DateTime.UtcNow;

            var jwt = new JwtSecurityToken(
                claims: identity.Claims,
                notBefore: now,
                expires: now.Add(_options.Expiration),
                signingCredentials: _options.SigningCredentials);

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new
            {
                access_token = encodedJwt,
                expires_in = (int)_options.Expiration.TotalSeconds,
            };

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        private Task<ClaimsIdentity> GetIdentity(HttpContext context, string username, string password)
        {
            //var user = AccountInMemory.ArrayAccount.FirstOrDefault(x => x.UserName.Equals(username) && x.Password.Equals(password));
            //var user = _userManager.Validate("S0001", username, password);
            var user = _userManager.GetUserDataByUserCode(username);
            if (user == null) return null;

            IList<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserID, null, ClaimsIdentity.DefaultIssuer, "Provider"));

            claims.Add(new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}", null, ClaimsIdentity.DefaultIssuer, "Provider"));

            claims.Add(new Claim("Username", user.UserName));

            return Task.FromResult(new ClaimsIdentity(claims, "Bearer"));
        }
    }
}
