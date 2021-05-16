using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ShowingAds.CoreLibrary.Managers;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Server.Controllers.Abstract
{
    //[Authorize]
    [Route("[controller]")]
    [ApiController]
    public abstract class ModelController<TKey, TModel, TModelManager> : ControllerBase
        where TKey : struct
        where TModel : class, ICloneable
        where TModelManager : IModelManager<TKey, TModel>
    {
        private TModelManager _manager { get; }
        private ILogger<ModelController<TKey, TModel, TModelManager>> _logger { get; }

        public ModelController(ILogger<ModelController<TKey, TModel, TModelManager>> logger, TModelManager manager)
        {
            _logger = logger;
            _manager = manager;
        }

        [HttpGet]
        public async Task<ActionResult> GetPermittedModels()
        {
            _logger.LogInformation($"Get models {HttpContext.User.Identity.Name} -> {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var manager = UserManager.GetInstance();
            var (isExists, user) = await manager.TryGetAsync(x => x.Name == HttpContext.User.Identity.Name);
            if (isExists == false)
                return StatusCode(StatusCodes.Status401Unauthorized);
            var users = await manager.GetEmployeeUsers(user.Id);
            var models = await _manager.GetPermittedModelsAsync(users);
            var json = JsonConvert.SerializeObject(models);
            return StatusCode(StatusCodes.Status200OK, json);
        }

        [HttpPost, HttpPut]
        public async Task<ActionResult> AddOrUpdateModel([FromBody] TModel model)
        {
            _logger.LogInformation($"Post model {HttpContext.User.Identity.Name} -> {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var isSuccess = await _manager.TryAddOrUpdateAsync(model);
            if (isSuccess)
                return StatusCode(StatusCodes.Status200OK);
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteModel([FromBody] JObject obj)
        {
            _logger.LogInformation($"Delete model {HttpContext.User.Identity.Name} -> {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var key = obj.Value<string>("id");
            TKey id = default;
            if (Guid.TryParse(key, out var result))
                id = (TKey)(object)result;
            else
                id = (TKey)(object)Convert.ToInt32(key);
            var (isSuccess, _) = await _manager.TryDeleteAsync(id);
            if (isSuccess)
                return StatusCode(StatusCodes.Status200OK);
            return StatusCode(StatusCodes.Status400BadRequest);
        }
    }
}
