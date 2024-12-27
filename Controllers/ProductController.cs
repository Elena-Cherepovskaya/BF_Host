using BF_Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace BF_Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ProductService _service;

        public ProductController(ILogger<ProductController> logger, ProductService service)
        {
            _logger = logger;
            _service = service;
        }

        [HttpGet(Name = "GetProduct")]
        public IEnumerable<Product> Get()
        {
            return _service.GetAll();
        }

        [HttpPut("[action]", Name = "AddProduct")]
        public void Add(Product value)
        {
            _service.Add(value);
        }

        [HttpPut("[action]", Name = "DeleteProduct")]
        public void Delete(Product value)
        {
            _service.Delete(value);
        }
    }
}
