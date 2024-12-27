using BF_Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace BF_Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserService _service;

        public UserController(ILogger<UserController> logger, UserService users)
        {
            _logger = logger;
            _service = users;
        }

        [HttpGet(Name = "GetUser")]
        public IEnumerable<User> Get()
        {
            return _service.GetAllUser();
        }

        [HttpPut("[action]", Name = "AddUser")]
        public void Add(UserLogin user)
        {
            _service.AddUser(user);
        }

        [HttpPut("[action]", Name = "DeleteUser")]
        public void Delete(User user)
        {
            _service.Delete(user);
        }
    }
}
