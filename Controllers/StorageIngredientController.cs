using BF_Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace BF_Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageIngredientController : ControllerBase
    {
        private readonly ILogger<StorageIngredientController> _logger;
        private readonly StorageIngredientService _service;

        public StorageIngredientController(ILogger<StorageIngredientController> logger, StorageIngredientService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet(Name = "GetStorageIngredient")]
        public IEnumerable<StorageIngredient> Get()
        {
            return _service.GetAll();
        }

        [HttpPut("[action]", Name = "SetStorageIngredient")]
        public void Set(StorageIngredient value)
        {
            _service.Set(value);
        }

        [HttpPut("[action]", Name = "AddStorageIngredient")]
        public void Add(StorageIngredient value)
        {
            _service.Add(value);
        }

        [HttpPut("[action]", Name = "DeleteStorageIngredient")]
        public void Delete(StorageIngredient value)
        {
            _service.Delete(value);
        }
    }
}
