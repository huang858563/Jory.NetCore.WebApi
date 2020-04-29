using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Jory.NetCore.WebApi.Controllers
{
    /// <summary>
    /// 健康檢查
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class HealthController : ControllerBase
    {
        private readonly ILogger<HealthController> _logger;

        public HealthController(ILogger<HealthController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 健康檢查方法
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("*****Health Check*****");
            return Ok();
        }
    }
}