using BF_Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace BF_Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StorageProductController : ControllerBase
    {
        private readonly ILogger<StorageProductController> _logger;
        private readonly StorageProductService _service;

        public StorageProductController(ILogger<StorageProductController> logger, StorageProductService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet(Name = "GetStorageProduct")]
        public IEnumerable<StorageProduct> Get()
        {
            return _service.GetAll();
        }

        [HttpPut("[action]", Name = "SetStorageProduct")]
        public void Set(StorageProduct value)
        {
            _service.Set(value);
        }

        [HttpPut("[action]", Name = "AddStorageProduct")]
        public void Add(StorageProduct value)
        {
            _service.Add(value);
        }

        [HttpPut("[action]", Name = "DeleteStorageProduct")]
        public void Delete(StorageProduct value)
        {
            _service.Delete(value);
        }
    }
}
