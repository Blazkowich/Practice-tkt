using EPM.TKT.App.DependencyInjections;
using EPM.TKT.External.DependencyInjections;
using EPM.TKT.Infrastructure.DependencyInjections;
using EPM.TKT.WebApi.Contracts.Requests.Auth;
using EPM.TKT.WebApi.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddMemoryCache();
builder.Services.AddResponseCaching();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterHttpRequest>();
builder.Services.AddOpenApi();

builder.Services.RegisterExternal();
builder.Services.RegisterInfrastructure();
builder.Services.RegisterApplication();

builder.Services.AddJwtAuthentication();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseResponseCaching();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();