using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShowingAds.Backend.UserService.Managers;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShowingAds.Backend.UserService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public abstract class BaseModelController<TKey, TModel, TModelManager> : Controller
        where TKey : struct
        where TModel : class, ICloneable
        where TModelManager : IModelManager<TKey, TModel>
    {
        private readonly TModelManager _manager;

        public BaseModelController(TModelManager manager) => _manager = manager;

        [HttpGet]
        public async Task<ActionResult> GetPermittedModels()
        {
            var manager = UserManager.GetInstance();
            var user = await manager.GetOrDefaultAsync(x => x.Name == User.Identity.Name);
            if (user == default)
                return Unauthorized();
            var users = await manager.GetEmployeeUsers(user.Id);
            var models = await _manager.GetPermittedModelsAsync(users);
            return Ok(JsonConvert.SerializeObject(models));
        }

        [HttpPost, HttpPut]
        public async Task<ActionResult> AddOrUpdateModel([FromBody] TModel model)
        {
            var isSuccess = await _manager.TryAddOrUpdateAsync(model);
            if (isSuccess)
                return Ok();
            return BadRequest();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteModel([FromBody] TModel model)
        {
            var isDeleted = await _manager.TryDeleteAsync(model);
            if (isDeleted)
                return Ok();
            return BadRequest();
        }
    }
}
