using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using server.Models;
using server.Services;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("sorted-by-completed-todos")]
        public async Task<ActionResult<List<ApplicationUserWithTodosDto>>> GetUsersSortedByCompletedTodosAsync()
        {
            var users = await _userService.GetUserSortedByCompletedTodosAsync();
            return Ok(users);
        }

        [HttpGet]
        public async Task<ActionResult<List<ApplicationUserDto>>> GetUsersAsync()
        {
            var users = await _userService.GetUsersAsync();
            return Ok(users);
        }
    }
}
