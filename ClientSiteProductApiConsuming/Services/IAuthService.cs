using ClientSiteProductApiConsuming.Models;

namespace ClientSiteProductApiConsuming.Services
{
    public interface IAuthService
    {
        Task<string> AuthenticateAsync(UserLogin loginModel);
    }
}
