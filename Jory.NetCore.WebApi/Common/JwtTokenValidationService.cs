using Jory.NetCore.WebApi.Models;
using Jory.NetCore.WebApi.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Jory.NetCore.Model.Entities;

namespace Jory.NetCore.WebApi.Common
{
    public class JwtTokenValidationService : IJwtTokenValidationService
    {
        private readonly JwtBearerOption _jwtBearerOption;

        public JwtTokenValidationService(IOptions<JwtBearerOption> jwtBearerOption)
        {
            _jwtBearerOption = jwtBearerOption.Value;
        }

        public string GenerateToken(ADUserT user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "mc00118@mail.com"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtBearerOption.SecurityKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtBearerOption.Issuer,
                _jwtBearerOption.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_jwtBearerOption.AccessExpiration),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> GenerateTokenAsync(ADUserT user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "mc00118@mail.com"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtBearerOption.SecurityKey));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _jwtBearerOption.Issuer,
                _jwtBearerOption.Audience,
                claims,
                expires: DateTime.Now.AddMinutes(_jwtBearerOption.AccessExpiration),
                signingCredentials: credentials
            );

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
    }
}
