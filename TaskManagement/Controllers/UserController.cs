using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagement.Interfaces;
using TaskManagement.Models;
using TaskManagement.Services;
using TaskManagement.DTOs;

namespace TaskManagement.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : Controller
    {
        private readonly IGenericRepository<User> _repository;
        private readonly IConfiguration _configuration;
        private readonly AuthService _authHelper;

        public UserController(IGenericRepository<User> repository, IConfiguration configuration, AuthService authHelper)
        {
            _repository = repository;
            _configuration = configuration;
            _authHelper = authHelper;
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] UserDto registerUser)
        {
            if (registerUser.Username == null || registerUser.Email == null || registerUser.Password == null)
            {
                return BadRequest("Username, email and password are required.");
            }
            if (_repository.Any(u => u.Username == registerUser.Username))
            {
                return Conflict("Username already exists");
            }
            if (_repository.Any(u => u.Email == registerUser.Email))
            {
                return Conflict("Email already exists");
            }
            try
            {
                User user = new User
                {
                    Username = registerUser.Username,
                    Email = registerUser.Email,
                    PasswordHash = SecurePassword.Hash(registerUser.Password)
                };
                _repository.Add(user);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] UserDto loginUser)
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
            bool isValid = SecurePassword.Verify(loginUser.Password, user.PasswordHash);
            if (!isValid)
            {
                return Unauthorized("Invalid username or password.");
            }

            string accessToken = _authHelper.CreateToken(user);
            Response.Cookies.Append("token", accessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddDays(1)
                //Expires = DateTime.UtcNow.AddMinutes(1)
            });

            //return Ok(new { token = accessToken });
            return Ok(new { message = "Login successfully" });
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
