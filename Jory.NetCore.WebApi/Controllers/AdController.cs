using Jory.NetCore.Core;
using Jory.NetCore.Model.Entities;
using Jory.NetCore.Model.Models;
using Jory.NetCore.WebApi.Common;
using Jory.NetCore.WebApi.Models;
using Jory.NetCore.WebApi.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace Jory.NetCore.WebApi.Controllers
{
    /// <summary>
    /// 基礎功能
    /// </summary>
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class AdController : ControllerBase
    {
        private readonly ILogger<AdController> _logger;
        private readonly IJwtTokenValidationService _jwtTokenValidationService;
        private readonly JwtBearerOption _jwtBearerOption;
        private readonly IDistributedCache _cache;

        public AdController(ILogger<AdController> logger, IJwtTokenValidationService jwtTokenValidationService,
            IOptions<JwtBearerOption> jwtBearerOption, IDistributedCache cache)
        {
            _logger = logger;
            _jwtTokenValidationService = jwtTokenValidationService;
            _cache = cache;
            _jwtBearerOption = jwtBearerOption?.Value ?? throw new ArgumentException();
        }

        [HttpPost("GetToken")]
        [AllowAnonymous]
        public IActionResult GetToken([FromBody] LoginModel model)
        {
            ResultModel resultModel;
            _logger.LogInformation($"{model.UserName}:{model.Password}");
            if (!string.IsNullOrWhiteSpace(model.UserName) && !string.IsNullOrWhiteSpace(model.Password))
            {
                var user = new ADUserT
                {
                    UserName = "admin",
                    Role = "admin"
                };

                var refreshToken = Guid.NewGuid().ToString("N");
                var refreshTokenExpiredTime = DateTime.Now.AddMinutes(_jwtBearerOption.RefreshExpiration);

                var cacheKey = $"RefreshToken:{refreshToken}";
                var cacheValue = JsonConvert.SerializeObject(user);

                _cache.SetString(cacheKey, cacheValue,
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpiration = refreshTokenExpiredTime
                    });

                resultModel = ResultModel.GetSuccess("", new
                {
                    AccessToken = _jwtTokenValidationService.GenerateToken(user),
                    RefreshTokenExpired = DateTimeHelper.ConvertToLong(refreshTokenExpiredTime),
                    RefreshToken = refreshToken
                });
            }
            else
            {
                resultModel = ResultModel.GetFail("用戶名或密碼錯誤");
            }

            return Ok(resultModel);
        }

        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken([FromForm] string refreshToken)
        {
            ResultModel resultModel;
            var cacheStr = _cache.GetString($"RefreshToken:{refreshToken}");
            if (string.IsNullOrWhiteSpace(cacheStr))
            {
                resultModel = ResultModel.GetFail("Token不存在或已过期");
                return Ok(resultModel);
            }

            var cacheUser = JsonConvert.DeserializeObject<ADUserT>(cacheStr);
            var userName = User.Claims.First(x => x.Type == JwtRegisteredClaimNames.UniqueName);

            if (userName == null || cacheUser.UserName != userName.Value)
            {
                resultModel = ResultModel.GetFail("用户不匹配");
                return Ok(resultModel);
            }

            var newToken = Guid.NewGuid().ToString("N");
            var cacheKey = $"RefreshToken:{newToken}";
            var refreshTokenExpiredTime = DateTime.Now.AddMinutes(_jwtBearerOption.RefreshExpiration);

            _cache.SetString(cacheKey, cacheStr, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = refreshTokenExpiredTime
            });

            resultModel = ResultModel.GetSuccess("", new
            {
                AccessToken = _jwtTokenValidationService.GenerateToken(cacheUser),
                RefreshTokenExpired = DateTimeHelper.ConvertToLong(refreshTokenExpiredTime),
                RefreshToken = refreshToken
            });

            return Ok(resultModel);
        }
    }
}