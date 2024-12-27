using BF_Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace BF_Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IngredientController : ControllerBase
    {
        private readonly ILogger<IngredientController> _logger;
        private readonly IngredientService _service;

        public IngredientController(ILogger<IngredientController> logger, IngredientService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet(Name = "GetIngredient")]
        public IEnumerable<Ingredient> Get()
        {
            return _service.GetAll();
        }

        [HttpPut("[action]", Name = "AddIngredient")]
        public void Add(Ingredient value)
        {
            _service.Add(value);
        }

        [HttpPut("[action]", Name = "DeleteIngredient")]
        public void Delete(Ingredient value)
        {
            _service.Delete(value);
        }
    }
}
