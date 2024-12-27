using BF_Host.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BF_Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientForRecipeController : ControllerBase
    {
        private readonly ILogger<IngredientForRecipeController> _logger;
        private readonly IngredientForRecipeService _service;

        public IngredientForRecipeController(ILogger<IngredientForRecipeController> logger, IngredientForRecipeService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpPost(Name = "GetIngredientForRecipe")]
        public IEnumerable<IngredientForRecipe> Get(Recipe value)
        {
            return _service.Get(value);
        }

        [HttpPut("[action]", Name = "AddIngredientForRecipe")]
        public void Add(IngredientForRecipe value)
        {
            _service.Add(value);
        }

        [HttpPut("[action]", Name = "SetIngredientForRecipe")]
        public void Set(IngredientForRecipe value)
        {
            _service.Set(value);
        }

        [HttpPut("[action]", Name = "DeleteIngredientForRecipe")]
        public void Delete(IngredientForRecipe value)
        {
            _service.Delete(value);
        }
    }
}
