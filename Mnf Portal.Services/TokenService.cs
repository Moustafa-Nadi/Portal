using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mnf_Portal.APIs.DTOs;
using Mnf_Portal.Core.Entities;
using Mnf_Portal.Core.Entities.Identity;
using Mnf_Portal.Core.Interfaces;
using Mnf_Portal.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Mnf_Portal.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly IMnfIdentityContextRepo<RefreshToken> _repo;
        private readonly UserManager<AppUser> _userManager;

        public TokenService(IConfiguration configuration, IMnfIdentityContextRepo<RefreshToken> repo, UserManager<AppUser> userManager)
        {
            _configuration = configuration;
            _repo = repo;
            _userManager = userManager;
        }

        public async Task<string> CreateTokenAsync(AppUser appUser)
        {
            // Private Claims [User Defined]

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.GivenName, $"{appUser.FirstName} {appUser.LastName}"),
                new(ClaimTypes.Email, appUser.Email!),
                // UserName
            };

            var roles = await _userManager.GetRolesAsync(appUser)!;
            foreach (var role in roles)
                authClaims.Add(new(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                // Payload  [Register Claims]
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(double.Parse(_configuration["JWT:DurationTime"]!)),
                // Private Claims
                claims: authClaims,

                signingCredentials: creds   // Signature
        );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> CreateRefreshToken(AppUser appUser)
        {
            var randomNumber = new byte[64];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            var token = Convert.ToBase64String(randomNumber);

            RefreshToken refreshToken = new()
            {
                Token = token,
                userId = appUser.Id,
                ExpDate = DateTime.UtcNow.AddDays(double.Parse(_configuration["JWT:DurationTime"]!))
            };

            var currentRefreshToken = (await _repo.GetAllAsync(rt => rt.userId == appUser.Id)).FirstOrDefault();

            if (currentRefreshToken is { })
            {
                currentRefreshToken.ExpDate = refreshToken.ExpDate;
                currentRefreshToken.Token = refreshToken.Token;

                await _repo.UpdateAsync(currentRefreshToken);
            }
            else
            {
                await _repo.CreateAsync(refreshToken);
            }

            return token;
        }

        public async Task<AuthServiceResponseDto> ValidateRefreshToken(string token)
        {
            var refreshToken = (await _repo.GetAllAsync(rt => rt.Token == token)).FirstOrDefault();

            if (refreshToken is null || refreshToken.ExpDate < DateTime.UtcNow)
            {
                return new AuthServiceResponseDto()
                {
                    Message = "Invalid token",
                    IsSucceed = false
                };
            }

            var user = await _userManager.FindByIdAsync(refreshToken.userId);

            return new()
            {
                Message = "succeeded",
                IsSucceed = true,
                AccessToken = await CreateTokenAsync(user),
                RefreshToken = await CreateRefreshToken(user)
            };
        }
    }
}
