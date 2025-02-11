using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.DirectoryServices.AccountManagement;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace DMSAudit.ApiService.Services;

public interface IAuthenticationService
{
    Task<TokenResponse?> AuthenticateAsync(string? windowsIdentity);
    string GenerateJwtToken(string username, IEnumerable<string> roles);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthenticationService> _logger;

    public AuthenticationService(IConfiguration configuration, ILogger<AuthenticationService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<TokenResponse?> AuthenticateAsync(string? windowsIdentity)
    {
        if (string.IsNullOrEmpty(windowsIdentity))
        {
            _logger.LogWarning("Authentication attempted with null or empty identity");
            return null;
        }

        try
        {
            // Get user roles from Active Directory
            var roles = await GetUserRolesFromADAsync(windowsIdentity);
            
            // Generate JWT token
            var token = GenerateJwtToken(windowsIdentity, roles);

            return new TokenResponse
            {
                Token = token,
                Expiration = DateTime.UtcNow.AddHours(1)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during authentication for user {User}", windowsIdentity);
            return null;
        }
    }

    public string GenerateJwtToken(string username, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username),
            new(ClaimTypes.NameIdentifier, username)
        };

        // Add roles to claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? 
                throw new InvalidOperationException("JWT Key not configured")));
        
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private async Task<IEnumerable<string>> GetUserRolesFromADAsync(string username)
    {
        #if WINDOWS
        try
        {
            using (var context = new PrincipalContext(ContextType.Domain))
            using (var user = UserPrincipal.FindByIdentity(context, username))
            {
                if (user != null)
                {
                    var groups = user.GetGroups();
                    return groups.Select(g => g.Name);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting AD roles for user {User}", username);
        }
        #endif

        // Default role if AD lookup fails or not on Windows
        return new[] { "User" };
    }
} 