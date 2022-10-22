using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Security
{
    public class AthunticationHandler : IAthunticationHandler
    {
        private readonly JwtSecurityTokenHandler tokenHandler;
        JwtConfig JwtConfig;
        SecurityKey SignSecurityKey;
        SigningCredentials Credentials;
        JwtHeader JwtHeaders;

        public TokenValidationParameters ValidationParameters { get; set; }

        public AthunticationHandler(IOptions<JwtConfig> configuration)
        {
            tokenHandler = new JwtSecurityTokenHandler();
            JwtConfig = configuration.Value;

            SignSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtConfig.SecrectKey));
            Credentials = new SigningCredentials(SignSecurityKey, SecurityAlgorithms.HmacSha256);
            JwtHeaders = new JwtHeader(Credentials);
            ValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidIssuer = JwtConfig.Issuer,
                IssuerSigningKey = SignSecurityKey
            };
        }
        public JwtAuthToken Create(string userId, IEnumerable<Claim>? claims= null)
        {
            var nowUtc = DateTime.UtcNow;
            var expires = nowUtc.AddMinutes(JwtConfig.ExpiryMinutes);
            var centuryBegin = new DateTime(1970, 1, 1).ToUniversalTime();
            var exp = (long)(new TimeSpan(expires.Ticks - centuryBegin.Ticks).TotalSeconds);
            var now = (long)(new TimeSpan(nowUtc.Ticks - centuryBegin.Ticks).TotalSeconds);

            var payload = new JwtPayload()
            {
                {"sub", userId },
                {"iss", JwtConfig.Issuer },
                {"iat", now },
                {"unique_name", userId },
                {"exp", exp  }
            };

            if (claims != null)
                payload.AddClaims(claims);

            var jwt = new JwtSecurityToken(JwtHeaders, payload);
            var token = tokenHandler.WriteToken(jwt);
            var jsonToken = new JwtAuthToken() { Token = token, Expires = exp };


            return jsonToken;
        }
    }
}
