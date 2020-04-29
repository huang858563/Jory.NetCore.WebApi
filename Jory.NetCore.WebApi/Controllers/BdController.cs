using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jory.NetCore.Model.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Jory.NetCore.WebApi.Controllers
{
    /// <summary>
    /// 基本資料
    /// </summary>
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BdController : ControllerBase
    {
        private readonly ILogger<BdController> _logger;

        public BdController(ILogger<BdController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            _logger.LogInformation("hello world");
            return Ok(ResultModel.GetSuccess("", DateTime.Now.ToString("yyyy-MM-dd")));
        }
    }
}