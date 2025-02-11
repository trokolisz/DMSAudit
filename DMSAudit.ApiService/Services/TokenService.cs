using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

public class TokenService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public TokenService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            // Configure HttpClient to use Windows Authentication
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Negotiate");
            
            var response = await _httpClient.GetFromJsonAsync<TokenResponse>(_configuration["ApiService:TokenUrl"]);
            return response?.Token;
        }
        catch
        {
            return null;
        }
    }
}

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
} 