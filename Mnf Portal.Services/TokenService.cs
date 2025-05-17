using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Mnf_Portal.Core.Entities.Identity;
using Mnf_Portal.Core.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Mnf_Portal.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration) { _configuration = configuration; }

        public async Task<string> CreateTokenAsync(AppUser appUser, UserManager<AppUser> userManager)
        {
            // Private Claims [User Defined]

            var authClaims = new List<Claim>
            {
                new(ClaimTypes.GivenName, $"{appUser.FirstName} {appUser.LastName}"),
                new(ClaimTypes.Email, appUser.Email!),
                // UserName
            };

            var roles = await userManager.GetRolesAsync(appUser)!;
            foreach (var role in roles)
                authClaims.Add(new(ClaimTypes.Role, role));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                // Payload  [Register Claims]
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.Now.AddDays(double.Parse(_configuration["JWT:DurationTime"]!)),
                // Private Claims
                claims: authClaims,

                signingCredentials: creds   // Signature
        );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
