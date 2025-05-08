using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using PortfolioTrackerApi.Models;

namespace PortfolioTrackerApi.Services
{
    public class FinnhubService : IFinnhubService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;

        public FinnhubService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["Finnhub:ApiKey"];
            _baseUrl = config["Finnhub:BaseUrl"];
        }

        public async Task<List<StockMatch>> SearchSymbolAsync(string query, string? exchange = null)
        {
            var url = $"{_baseUrl}/search?q={query}&token={_apiKey}";
            if (!string.IsNullOrWhiteSpace(exchange))
                url += $"&exchange={exchange}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            var results = new List<StockMatch>();
            foreach (var item in data.GetProperty("result").EnumerateArray())
            {
                results.Add(new StockMatch
                {
                    Symbol = item.GetProperty("symbol").GetString(),
                    Name = item.GetProperty("description").GetString(),
                    Type = item.GetProperty("type").GetString(),
                    Region = item.GetProperty("displaySymbol").GetString()
                });
            }

            return results;
        }

        public async Task<StockQuote> GetQuoteAsync(string symbol)
        {
            var url = $"{_baseUrl}/quote?symbol={symbol}&token={_apiKey}";
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<JsonElement>(json);

            decimal GetDecimalSafe(JsonElement element, string propertyName)
            {
                if (element.TryGetProperty(propertyName, out JsonElement prop) && 
                    prop.ValueKind == JsonValueKind.Number)
                {
                    return prop.GetDecimal();
                }
                return 0m; // fallback default
            }

            return new StockQuote
            {
                Symbol = symbol,
                CurrentPrice = GetDecimalSafe(data, "c"),
                OpenPrice = GetDecimalSafe(data, "o"),
                GainLoss = GetDecimalSafe(data, "d"),
                GainLossPercentage = GetDecimalSafe(data, "dp")
            };
        }
    }
}
