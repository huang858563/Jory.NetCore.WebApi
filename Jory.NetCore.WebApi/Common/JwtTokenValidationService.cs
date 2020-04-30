using Jory.NetCore.Model.Entities;
using Jory.NetCore.WebApi.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
                new Claim(JwtRegisteredClaimNames.Sub, user.LoginName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture),ClaimValueTypes.Integer64),
                //new Claim(JwtRegisteredClaimNames.Email, user.Email),
                //用户名
                new Claim(ClaimTypes.Name,user.LoginName),
                //角色
                new Claim(ClaimTypes.Role,user.Role)
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
                new Claim(JwtRegisteredClaimNames.Sub, user.LoginName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToUniversalTime().ToString(CultureInfo.InvariantCulture),ClaimValueTypes.Integer64),
                //new Claim(JwtRegisteredClaimNames.Email, user.Email),
                //用户名
                new Claim(ClaimTypes.Name,user.LoginName),
                //角色
                new Claim(ClaimTypes.Role,user.Role)
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
