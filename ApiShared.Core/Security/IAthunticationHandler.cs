using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ApiShared.Core.Security
{
    public interface IAthunticationHandler
    {
        JwtAuthToken Create(string userId, IEnumerable<Claim>? claims = null);
        TokenValidationParameters ValidationParameters { get; set; }
    }
}
