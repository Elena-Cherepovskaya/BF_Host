using BF_Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace BF_Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipeController : ControllerBase
    {
        private readonly ILogger<RecipeController> _logger;
        private readonly RecipeService _service;

        public RecipeController(ILogger<RecipeController> logger, RecipeService service)
        {
            _logger = logger;
            _service = service;
        }

        //api/Recipe
        [HttpGet(Name = "GetRecipe")]
        public IEnumerable<Recipe> Get()
        {
            return _service.GetAll();
        }

        //api/Recipe/Add
        [HttpPut("[action]", Name = "AddRecipe")]
        public void Add(Recipe value)
        {
            _service.Add(value);
        }

        [HttpPut("[action]", Name = "DeleteRecipe")]
        public void Delete(Recipe value)
        {
            _service.Delete(value);
        }

        [HttpPost("[action]", Name = "UseRecipe")]
        public bool Use(Recipe value)
        {
            return _service.Use(value);
        }
    }
}
