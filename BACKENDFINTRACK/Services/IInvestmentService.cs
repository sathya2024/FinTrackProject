using PortfolioTrackerApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
 
namespace PortfolioTrackerApi.Services
{
    public interface IInvestmentService
    {
        Task<IEnumerable<Investment>> GetAllAsync();
        Task<IEnumerable<Investment>> GetInvestmentsByUserAsync(int UserId);
        Task AddInvestmentAsync(Investment investment);
    }
}