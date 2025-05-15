using Microsoft.AspNetCore.Identity;
using Mnf_Portal.Core.Entities.Identity;

namespace Mnf_Portal.Core.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser appUser, UserManager<AppUser> userManager);
    }
}
