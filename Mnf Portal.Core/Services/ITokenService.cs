using Mnf_Portal.APIs.DTOs;
using Mnf_Portal.Core.Entities.Identity;

namespace Mnf_Portal.Core.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser appUser);
        Task<string> CreateRefreshToken(AppUser appUser);
        Task<AuthServiceResponseDto> ValidateRefreshToken(string token);
    }
}
