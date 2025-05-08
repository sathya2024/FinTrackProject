using System.Text.Json;
using PortfolioTrackerApi.Models;
using PortfolioTrackerApi.Services;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
 
namespace PortfolioTrackerApi.Services
{
    public class InvestmentService : IInvestmentService
    {
        private readonly string _jsonPath = "Data/investments.json";
 
        public async Task AddInvestmentAsync(Investment investment)
        {
            var investments = await LoadInvestmentsAsync();
            investment.Id = Guid.NewGuid().ToString(); // Generate a unique string ID
           
            investments.Add(investment);
           
            var options = new JsonSerializerOptions { WriteIndented = true, Converters = { new InvestmentJsonConverter() } };
            var json = JsonSerializer.Serialize(investments, options);
            await File.WriteAllTextAsync(_jsonPath, json);
        }
 
        public async Task<List<Investment>> LoadInvestmentsAsync()
        {
            if (!File.Exists(_jsonPath)) return new List<Investment>();
            var json = await File.ReadAllTextAsync(_jsonPath);
            return JsonSerializer.Deserialize<List<Investment>>(json, new JsonSerializerOptions
            {
                Converters = { new InvestmentJsonConverter() }
            }) ?? new List<Investment>();
        }
 
        public async Task<IEnumerable<Investment>> GetAllAsync()
        {
            return await LoadInvestmentsAsync();
        }
 
        public async Task<IEnumerable<Investment>> GetInvestmentsByUserAsync(int UserId)
        {
            var investments = await LoadInvestmentsAsync();
            return investments.Where(i => i.UserId == UserId);
        }
    }
}
 