using PortfolioTrackerApi.Models;

namespace PortfolioTrackerApi.Services
{
    public interface IUserService
    {
        Task<User?> ValidateCredentialsAsync(string userName, string password);
        Task<bool> RegisterUserAsync(User newUser); 
    }
}
