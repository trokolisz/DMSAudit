using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using DMSAudit.ApiService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DMSAudit.ApiService.Endpoints;

public static class TokenEndpoints
{
    public static void MapTokenEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/token", async (
            HttpContext context,
            [FromServices] IAuthenticationService authService,
            [FromServices] ILogger<Program> logger) =>
        {
            try
            {
                // Get Windows identity
                var windowsIdentity = context.User?.Identity?.Name;
                
                if (string.IsNullOrEmpty(windowsIdentity))
                {
                    logger.LogWarning("No Windows identity found in request");
                    return Results.Unauthorized();
                }

                var tokenResponse = await authService.AuthenticateAsync(windowsIdentity);
                
                if (tokenResponse == null)
                {
                    logger.LogWarning("Authentication failed for user {User}", windowsIdentity);
                    return Results.Unauthorized();
                }

                return Results.Ok(tokenResponse);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Token generation failed");
                return Results.Problem(
                    title: "Authentication Failed",
                    detail: "An error occurred during authentication",
                    statusCode: StatusCodes.Status500InternalServerError
                );
            }
        })
        .RequireAuthorization()  // Require Windows Authentication
        .WithName("GetToken")
        .Produces<TokenResponse>(contentType: "application/json")
        .Produces(StatusCodes.Status401Unauthorized)
        .Produces(StatusCodes.Status500InternalServerError)
        .WithOpenApi();
    }
}

public class TokenResponse
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
} 