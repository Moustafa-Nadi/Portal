using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Mnf_Portal.APIs.DTOs;
using Mnf_Portal.Core.Entities.Identity;
using Mnf_Portal.Core.Services;
using System.Net;

namespace Mnf_Portal.APIs.Controllers
{
    public class AccountsController : ApiBaseController
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;

        public AccountsController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("login")] // POST : /api/accounts/ login
        public async Task<ActionResult<UserDto>> Login(LoginDto userModel)
        {
            var user = await _userManager.FindByEmailAsync(userModel.Email);
            if (user is null)
                return Unauthorized("Authorized, You are not!");
            var result = await _signInManager.CheckPasswordSignInAsync(user, userModel.Password, false);
            if (!result.Succeeded)
                return Unauthorized("Authorized, You are not!");

            return Ok(
                new UserDto
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    Token = await _tokenService.CreateTokenAsync(user, _userManager)
                });
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {
            if (CheckEmailExists(model.Email).Result.Value)
                return BadRequest("This Email already used!!");
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
                return BadRequest(HttpStatusCode.BadRequest);

            await _userManager.AddToRoleAsync(user, "User"); //

            return Ok(
                new UserDto
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Token = await _tokenService.CreateTokenAsync(user, _userManager)
                });
        }

        [HttpGet("emailExists")] // GET : / api/accounts/emailExists?email = SaraMohammed@gmail.com
        public async Task<ActionResult<bool>> CheckEmailExists(string email) => await _userManager.FindByEmailAsync(email) is not null;
    }
}
