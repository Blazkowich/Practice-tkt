using System.Text;
using EPM.TKT.External;
using EPM.TKT.Infrastructure.DependencyInjections;
using EPM.TKT.Mediator.DependencyInjection;
using EPM.TKT.App;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.RegisterExternal();
builder.Services.RegisterInfrastructure();
builder.Services.AddMediator<AppAssemblyMarker>();

var jwtSecret = builder.Configuration["Jwt:Secret"]
    ?? throw new InvalidOperationException("Jwt:Secret is not configured.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey        = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidIssuer             = builder.Configuration["Jwt:Issuer"],
            ValidAudience           = builder.Configuration["Jwt:Audience"],
            ValidateLifetime        = true,
            ClockSkew               = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
