using BF_Host.Services;
using Microsoft.AspNetCore.Mvc;

namespace BF_Host.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserService _users;

        public LoginController(ILogger<UserController> logger, UserService users)
        {
            _logger = logger;
            _users = users;
        }
       
        [HttpPost(Name = "Login")]
        public IEnumerable<User> Post(UserLogin user)
        {
            List<User> result = _users.TryLoginUser(user);
            return result;
        }
    }
}
