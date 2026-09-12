using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceLayer.Interfaces;

namespace UserAPI.Controllers
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

        /// <summary>
        /// Gets lists of users
        /// </summary>
        /// <returns>
        /// Success: List of Users
        /// Failure: 404 Not Found Error
        /// </returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();

            if(users == null || !users.Any())
                return NotFound();           

            return Ok(users);
        }

        /// <summary>
        /// Gets a user by ID
        /// </summary>
        /// <param name="id">Primary key of the user to retrieve</param>
        /// <returns>
        /// Success: User object for the given ID
        /// Failure: 404 Not Found Error
        /// </returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(long id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if(user.Id == 0)
                return NotFound();

            return Ok(user);
        }
    }
}
