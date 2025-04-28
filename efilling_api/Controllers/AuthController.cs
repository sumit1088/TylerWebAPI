using Microsoft.AspNetCore.Mvc;
using efilling_api.Services;
using BCrypt.Net;
using efilling_api.Models;
using System;
using Microsoft.EntityFrameworkCore;

namespace efilling_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly EFilling_DBContext _context;
        private readonly JwtService _jwtService;

        public AuthController(EFilling_DBContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        //[HttpPost("register")]
        //public IActionResult Register(UserLogin dto)
        //{
        //    var user = new UserInfo
        //    {
        //        Email = dto.Username,
        //        PasswordEncrypted = BCrypt.Net.BCrypt.HashPassword(dto.Password)
        //    };

        //    _context.UserInfos.Add(user);
        //    _context.SaveChanges();

        //    return Ok(new { message = "User registered successfully." });
        //}

        [HttpPost("login")]
        public IActionResult Login(LoginRequest request)
        {
            LoginResponse resp = new LoginResponse();

            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
            {
                resp.success = false;
                resp.status = 400;
                resp.message = "Username and password are required.";
                return Ok(resp);
                //return BadRequest("Username and password are required.");
            }

            var user = _context.UserInfos.SingleOrDefault(u => u.Email == request.Username && u.PasswordEncrypted == request.Password);

            if (user == null)
            {
                resp.success = false;
                resp.status = 401;
                resp.message = "Invalid username or password.!";
                return Ok(resp);
                //return Unauthorized(new { message = "Invalid credentials." });
            }

            if (user != null)
            {
                var token = _jwtService.GenerateToken(user.Email);
                resp.success = true;
                resp.status = 200;
                resp.message = "Login successful!";
                resp.access_token = token;
                return Ok(resp);
            }

            return NoContent();
            //return Ok(new { access_token = token });
        }

        
        public class UserLogin
        {
            public int Id { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
        }

        public class LoginRequest
        {
            public int Id { get; set; }
            public string? Username { get; set; }
            public string? Password { get; set; }
        }
                
    }
}



//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] LoginRequest request)
//        {
//            CommonResponse resp = new CommonResponse();            

//            if (request == null || string.IsNullOrEmpty(request.Username) || string.IsNullOrEmpty(request.Password))
//            {
//                resp.success = false;
//                resp.status = 400;
//                resp.message = "Username and password are required.";
//                return Ok(resp);
//                //return BadRequest("Username and password are required.");
//            }

//            // Find the user by username
//            var user = await _context.UserInfos
//                .Where(u => u.Email == request.Username)
//                .FirstOrDefaultAsync();

//            //var user = await _context.UserInfos.FirstOrDefaultAsync(u => u.Email == request.Username && u.PasswordEncrypted == request.Password);

//            //if (user != null)
//            //{
//            //    var token = GenerateJwtToken(user.Email);
//            //    resp.success = true;
//            //    resp.status = 200;
//            //    resp.message = "Login successful!";
//            //    return Ok(new { resp, token });
//            //}

//            if (user == null)
//            {
//                resp.success = false;
//                resp.status = 401;
//                resp.message = "Invalid username or password.!";
//                return Ok(resp);
//                //return Unauthorized("Invalid username or password.");
//            }

//            // Hash the entered password and compare with the stored hash
//            if(request.Password == user.PasswordEncrypted)
//            {
//                resp.success = true;
//                resp.status = 200;
//                resp.message = "Login successful!";
//                return Ok(resp);
//                //return Ok(new { message = "Login successful" });
//            }
//            else
//            {
//                resp.success = false;
//                resp.status = 401;
//                resp.message = "Invalid username or password.!";
//                return Ok(resp);
//                //return Unauthorized("Invalid username or password.");
//            }
//            //bool isPasswordValid = PasswordHasher.VerifyPassword(request.Password, user.PasswordEncrypted, HashAlgorithmName.SHA512);

//            //if (!isPasswordValid)
//            //{
//            //    return Unauthorized("Invalid username or password.");
//            //}

//            // Return success or generate token (e.g., JWT)
//            //return Ok(new { message = "Login successful" });

//        }

