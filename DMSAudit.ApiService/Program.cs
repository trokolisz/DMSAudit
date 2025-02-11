using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using DMSAudit.ApiService.Models;
using DMSAudit.ApiService.Data;
using DMSAudit.ApiService.Endpoints;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Negotiate;
using DMSAudit.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add DbContext configuration
builder.Services.AddDbContext<DmsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TicketApp API",
        Version = "v1",
        Description = "API for managing tickets and users in the TicketApp system",
        Contact = new OpenApiContact
        {
            Name = "API Support",
            Email = "support@ticketapp.com"
        }
    });

    // Add JWT Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});




builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.SetMinimumLevel(LogLevel.Information);
});

// Add Windows Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = NegotiateDefaults.AuthenticationScheme;
})
.AddNegotiate()
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "DMSAudit",
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "DMSAudit",
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? 
                throw new InvalidOperationException("JWT Key not found in configuration")))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = options.DefaultPolicy;
});

// Add this with your other service registrations
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

var app = builder.Build();


app.MapCriteriaEndpoints();
app.MapStatusEndpoints();
// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseSwagger();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "TicketApp API V1");
    options.RoutePrefix = "swagger";
    options.DocumentTitle = "TicketApp API Documentation";
});
}
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();

// Add the token endpoints
app.MapTokenEndpoints();

app.MapDefaultEndpoints();

app.Run();
