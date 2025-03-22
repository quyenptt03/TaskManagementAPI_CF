using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Interfaces;
using TaskManagement.Models;
using TaskManagement.Services;
using TaskManagement.DTOs;
using Microsoft.AspNetCore.Identity;
using Azure.Core;
using AutoMapper;
using System.Data;

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
        private readonly IMapper _mapper;

        public UserController(IGenericRepository<User> repository,
           IConfiguration configuration,
           AuthService authHelper,
           UserManager<User> userManager,
           SignInManager<User> signinManager,
            IMapper mapper
            )
        {
            _repository = repository;
            _configuration = configuration;
            _authHelper = authHelper;
            _userManager = userManager;
            _signInManager = signinManager;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto registerUser)
        {

            if (registerUser.Email == null || registerUser.Password == null)
            {
                return BadRequest(new { message = "Email and password are required." });
            }
            var userData = _mapper.Map<User>(registerUser);

            bool isFirstUser = !_userManager.Users.Any();
            var user = new User { UserName = userData.Email, Email = userData.Email };
            var result = await _userManager.CreateAsync(user, registerUser.Password);

            string role = isFirstUser ? "Admin" : "User";

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);

                return Ok(new 
                    { data = new { UserName = user.UserName, Email = user.Email, Role = role },
                        message = "User registered successfully" 
                    });
            }

            return BadRequest(new { message = "Registration failed" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto loginUser)
        {
            if (loginUser.Email == null || loginUser.Password == null)
            {
                return BadRequest(new { message = "Email and password are required." });
            }

            var users = await _repository.GetAll();
            var user = users.FirstOrDefault(u => u.Email == loginUser.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }
            //var result = await _signInManager.PasswordSignInAsync(loginUser.Email,
            //               loginUser.Password, true, lockoutOnFailure: true);

            var result = await _signInManager.CheckPasswordSignInAsync(user, loginUser.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid email or password" });

            var role = await _userManager.GetRolesAsync(user);

            var token = _authHelper.CreateToken(user, role);
            Response.Cookies.Append("token", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(1)
            });

            return Ok(new
            { 
                message = "Logged in successfully",
                data = new { Id=user.Id, UserName = user.UserName, Email = user.Email, Role = role },
                accessToken = token
            });
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            Response.Cookies.Delete("token");
            return Ok(new { message = "Logged out successfully" });
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _repository.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetUserById([FromRoute] int id)
        {
            User user = await _repository.GetById(id);
            return user == null ? NotFound(new { message = "User not found" }) : Ok(user);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser([FromRoute] int id)
        {
            var user = await _repository.GetById(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found!!!!!" });
            }

            try
            {
                await _repository.Delete(id);
                return Ok(new {message = "User Deleted Successfully" });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }
    }
}
