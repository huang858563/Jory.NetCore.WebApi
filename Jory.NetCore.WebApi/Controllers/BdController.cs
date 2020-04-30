using Jory.NetCore.Model.Entities;
using Jory.NetCore.Repository.IRepositories.BD;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Jory.NetCore.Core.Helpers;
using Jory.NetCore.WebApi.Common;
using Jory.NetCore.WebApi.Resources;
using Microsoft.Extensions.Localization;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Jory.NetCore.WebApi.Controllers
{
    /// <summary>
    /// 基本資料
    /// </summary>
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BdController : ControllerBase
    {
        private readonly ILogger<BdController> _logger;
        private readonly IUserRep _userRep;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer _localizer;

        public BdController(ILogger<BdController> logger, IUserRep userRep, IMapper mapper,IStringLocalizer localizer)
        {
            _logger = logger;
            _userRep = userRep;
            _mapper = mapper;
            _localizer = localizer;
        }

        /// <summary>
        /// 无参GET请求
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ADUserT>), Status200OK)]
        public async Task<ActionResult> Get()
        {
            var userList = await _userRep.FindListAsync<ADUserT>();
            return Ok(userList);
        }

        /// <summary>
        /// 有参GET请求
        /// </summary>
        /// <param name="id">用户编号id</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ADUserT), Status200OK)]
        [ProducesResponseType(typeof(string), Status404NotFound)]
        public async Task<ActionResult> Get(int id)
        {
            var user = await _userRep.FindEntityAsync<ADUserT>(id);
            if (user != null) return Ok(user);
            return NotFound(_localizer["N00001"]);
        }

        /// <summary>
        /// PUT请求，新增/覆盖一条数据。
        /// </summary>
        /// <param name="value">用户JSON对象</param>
        /// <returns>是否执行成功</returns>
        [HttpPut]
        [ProducesResponseType(Status200OK)]
        [ProducesResponseType(typeof(string), Status400BadRequest)]
        public async Task<ActionResult> Put([FromBody] ADUserT value)
        {
            if (string.IsNullOrWhiteSpace(value.LoginPwd)) return BadRequest("Invalid password.");
            value.Role = ADUserT.GetRole(value.Role);
            value.LoginPwdHash = PasswordStorage.CreateHash(value.LoginPwd);
            var user = await _userRep.FindEntityAsync<ADUserT>(x => x.LoginName == value.LoginName);
            if (user != null)
            {
                //user = MapperHelper<ADUserT, ADUserT>.MapTo(value);
                //user = _mapper.Map<ADUserT>(value);
                await _userRep.UpdateAsync(value);
                return Ok();
            }

            await _userRep.InsertAsync(value);
            return Ok();
        }

        /// <summary>
        /// Delete请求，删除一条数据
        /// </summary>
        /// <param name="id">删除数据记录的id</param>
        /// <returns>是否执行成功</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(string), Status204NoContent)]
        [ProducesResponseType(typeof(string), Status404NotFound)]
        public async Task<ActionResult> Delete(string id)
        {
            var user = await _userRep.FindEntityAsync<ADUserT>(id);
            if (user != null)
            {
                await _userRep.DeleteAsync(user);
                return NoContent();
            }

            return NotFound("Cannot find key.");
        }
    }
}