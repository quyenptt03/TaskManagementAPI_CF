using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Interfaces;
using TaskManagement.Models;
using TaskManagement.Services;
using TaskManagement.DTOs;
using Microsoft.AspNetCore.Identity;
using Azure.Core;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IGenericRepository<User> _repository;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authHelper;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserController(IGenericRepository<User> repository,
           IConfiguration configuration,
           AuthService authHelper,
           UserManager<User> userManager,
           SignInManager<User> signinManager)
        {
            _repository = repository;
            _configuration = configuration;
            _authHelper = authHelper;
            _userManager = userManager;
            _signInManager = signinManager;
            //_roleManager = roleManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto registerUser)
        {
            if (registerUser.Email == null || registerUser.Password == null)
            {
                return BadRequest("Email and password are required.");
            }

            bool isFirstUser = !_userManager.Users.Any();
            var user = new User { UserName = registerUser.Email, Email = registerUser.Email };
            var result = await _userManager.CreateAsync(user, registerUser.Password);

            string role = isFirstUser ? "Admin" : "User";

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                return Ok(new { data = new { UserName = user.UserName, Email = user.Email, Role = role },
                    message = "User registered successfully" });
            }

            return BadRequest("Registration failed");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto loginUser)
        {
            if (loginUser.Email == null || loginUser.Password == null)
            {
                return BadRequest("Email and password are required.");
            }
            var user = _repository.GetAll().FirstOrDefault(u => u.Email == loginUser.Email);
            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginUser.Password, false);
            if (!result.Succeeded)
                return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);

            var token = _authHelper.CreateToken(user, roles);
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(1)
                //Expires = DateTime.UtcNow.AddMinutes(1)
            });

            return Ok("Login successfully");
        }

        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("token");
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpGet]
        public ActionResult GetUsers()
        {
            var result = (List<User>)_repository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult GetUserById([FromRoute] int id)
        {
            User user = _repository.GetById(id);
            return user == null ? NotFound("User not found") : Ok(user);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult DeleteUser([FromRoute] int id)
        {
            var user = _repository.GetById(id);
            if (user == null)
            {
                return NotFound("User not found!!!!!");
            }

            try
            {
                _repository.Delete(id);
                return Ok("User Deleted Successfully");
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }
    }
}
