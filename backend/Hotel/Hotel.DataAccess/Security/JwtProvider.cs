using Hotel.Domain.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using System.Text;
using System.Security.Claims;
using Hotel.Domain.Enums;
using Hotel.Domain.Abstractions.Security;

namespace Hotel.DataAccess.Security
{
        public class JwtProvider : IJwtProvider
        {
            private readonly JwtOptions _options;
            public JwtProvider(IOptions<JwtOptions> options)
            {
                _options = options.Value;

            }
            public string GenerateToken(User user)
            {
                List<Claim> claims = new List<Claim>()
                {
                   new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                   new Claim(ClaimTypes.Name, user.UserName),
                   new Claim(ClaimTypes.Role, user.Role.ToString())
                };
                var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SecretKey)),
                    SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    claims: claims,
                    signingCredentials: signingCredentials,
                    expires: DateTime.UtcNow.AddHours(_options.ExpiresHours));
                var tokenValue = new JwtSecurityTokenHandler().WriteToken(token);
                return tokenValue;
            }
        }
    }
