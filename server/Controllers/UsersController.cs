using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using server.Models;
using server.Services;

namespace server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMemoryCache _cache;

        public UsersController(IUserService userService, IMemoryCache cache)
        {
            _userService = userService;
            _cache = cache;
        }

        [HttpGet("sorted-by-completed-todos")]
        public async Task<ActionResult<List<ApplicationUserWithTodosDto>>> GetUsersSortedByCompletedTodosAsync()
        {
            var users = await _userService.GetUsersSortedByCompletedTodosAsync();
            return Ok(users);
        }

        [HttpGet("sorted-by-recent-activity")]
        public async Task<ActionResult<List<ApplicationUserWithTodosDto>>> GetUsersSortedByRecentActivityAsync()
        {
            var users = await _userService.GetUsersSortedByRecentActivityAsync();
            return Ok(users);
        }

        [HttpGet("sorted-by-task-difference")]
        public async Task<ActionResult<List<ApplicationUserWithTodosDto>>> GetUsersSortedByTaskDifferenceAsync()
        {
            var users = await _userService.GetUsersSortedByTaskDifferenceAsync();
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
