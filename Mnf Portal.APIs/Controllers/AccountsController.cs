using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mnf_Portal.APIs.Errors;
using Mnf_Portal.Core.DTOs;
using Mnf_Portal.Core.Entities;
using Mnf_Portal.Core.Entities.Identity;
using Mnf_Portal.Core.Interfaces;
using Mnf_Portal.Core.OtherObjects;
using Mnf_Portal.Core.Services;
using System.Data;
using System.Net;
using System.Runtime.InteropServices;

namespace Mnf_Portal.APIs.Controllers
{
    public class AccountsController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;
        private readonly EmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IMnfIdentityContextRepo<RefreshToken> _repo;

        public AccountsController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService,
            EmailService emailService,
            RoleManager<IdentityRole> roleManager, 
            IConfiguration config,
            IMnfIdentityContextRepo<RefreshToken> repo
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _config = config;
            _repo = repo;
        }

        [HttpPost("login")] // POST : /api/accounts/ login
        public async Task<ActionResult<UserDto>> Login(LoginDto userModel)
        {
            var user = await _userManager.FindByEmailAsync(userModel.Email);

            if (user is null)
                return Unauthorized(new ApiResponse(401, "Authorized, You are not!"));

            var result = await _signInManager.CheckPasswordSignInAsync(user, userModel.Password, false);

            if (!result.Succeeded)
                return Unauthorized(new ApiResponse(401, "Authorized, You are not!"));

            var accessToken = await _tokenService.CreateTokenAsync(user);
            var refreshToken = await _tokenService.CreateRefreshToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            var accessTokenCookie = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["JWT:DurationTime"]!))
            };

            var refreshTokenCookie = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(double.Parse(_config["JWT:DurationTime"]!))
            };

            Response.Cookies.Append("accessToken", accessToken, accessTokenCookie);
            Response.Cookies.Append("refreshToken", refreshToken, refreshTokenCookie);

            return Ok(new
            {
                userName = user.UserName,
                userId = user.Id,
                userRole = roles
            });
        }


        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register([FromBody]RegisterDto model)
        {
            if (await CheckEmailExists(model.Email))
                return BadRequest(new ApiResponse(400, "the email is already token"));

            if (model.Password != model.ComfimredPassword)
                return BadRequest(new ApiResponse(400, "Password do not match!"));

            var user = new AppUser
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                UserName = model.Email.Split('@')[0],
                PhoneNumber = model.PhoneNumber
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                string errorMessage = "User Creation Failed Because: ";

                foreach (var error in result.Errors)
                {
                    errorMessage += $"# {error.Description}";
                }

                return BadRequest(new ApiResponse(400, errorMessage));
            }

            await _userManager.AddToRoleAsync(user, StaticUserRoles.USER);

            var accessToken = await _tokenService.CreateTokenAsync(user);
            var refreshToken = await _tokenService.CreateRefreshToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            var accessTokenCookie = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["JWT:DurationTime"]!))
            };

            var refreshTokenCookie = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(double.Parse(_config["JWT:DurationTime"]!))
            };

            Response.Cookies.Append("accessToken", accessToken, accessTokenCookie);
            Response.Cookies.Append("refreshToken", refreshToken, refreshTokenCookie);

            return Ok(new
            {
                userName = user.UserName,
                userId = user.Id,
                userRole = roles
            });
        }


        [HttpPost("refresh")]
        public async Task<ActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (refreshToken is null) { return Unauthorized("invalid refresh token!"); }

            var result = await _tokenService.ValidateRefreshToken(refreshToken);
            if (!result.IsSucceed) { return Unauthorized("invalid refresh token!"); }

            var accessTokenCookie = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_config["JWT:DurationTime"]!))
            };

            var refreshTokenCookie = new CookieOptions()
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddDays(double.Parse(_config["JWT:DurationTime"]!))
            };

            Response.Cookies.Append("accessToken", result.AccessToken, accessTokenCookie);
            Response.Cookies.Append("refreshToken", result.RefreshToken, refreshTokenCookie);

            return Ok("Tokens refreshed");
        }


        [HttpPost("forgot-password")] // POST : /api/accounts/ forgot-password
        public async Task<ActionResult<string>> ForgetPassword([FromBody] ForgotPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return Ok("If the email exists, a reset link will be sent.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = Uri.EscapeDataString(token);
            var resetLink = $"https://your-frontend-url/reset-password?email={model.Email}&token={encodedToken}";

            await _emailService.SendEmailAsync(
                to: model.Email,
                subject: "Reset Your Password",
                body: $"<p>Click <a href='{resetLink}'>here</a> to reset your password.</p>"
            );

            return Ok("Reset password link sent.");
        }


        [HttpPost("reset-password")]  // POST : /api/accounts/ reset-password
        public async Task<ActionResult<string>> ResetPassword([FromBody] ResetPasswordDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user is null)
                return BadRequest(new ApiResponse(400, "Bad Request, you have made!"));

            var decodedToken = Uri.UnescapeDataString(model.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, model.NewPassword);

            if (!result.Succeeded)
                return BadRequest(new ApiResponse(400, "Bad Request, you have made!"));

            return Ok("Password reset successful.");
        }


        [HttpPost("make-admin")]
        public async Task<ActionResult> MakeAdmin([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByEmailAsync(updatePermissionDto.Email);

            if (user is null) { return NotFound("user does not exist!"); }

            await _userManager.AddToRoleAsync(user, StaticUserRoles.ADMIN);

            return Ok("done");
        }


        [HttpPost("make-owner")]
        public async Task<ActionResult> MakeOwner([FromBody] UpdatePermissionDto updatePermissionDto)
        {
            var user = await _userManager.FindByEmailAsync(updatePermissionDto.Email);

            if (user is null) { return NotFound("user does not exist!"); }

            await _userManager.AddToRoleAsync(user, StaticUserRoles.OWNER);

            return Ok("done");
        }


        [HttpPost]
        [Route("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("accessToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Ok(new { message = "Logged out successfully" });
        }


        [HttpGet("emailExists")] // GET : / api/accounts/emailExists?email = SaraMohammed@gmail.com
        public async Task<bool> CheckEmailExists(string email) => await _userManager.FindByEmailAsync(email) is not null;


        [HttpGet]
        [Route("get-profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var cookieRefreshToken = Request.Cookies["refreshToken"];
            var userRefreshToken = (await _repo.GetAllAsync(token => token.Token == cookieRefreshToken, Includes: token => token.AppUser)).FirstOrDefault();

            if (userRefreshToken is null || userRefreshToken.ExpDate < DateTime.UtcNow) { return Unauthorized(); }

            var user = userRefreshToken.AppUser;
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                userName = user.UserName,
                userId = user.Id,
                userRole = roles
            });
        }
    }
}