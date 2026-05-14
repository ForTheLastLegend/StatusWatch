using System.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Npgsql;
using StatusWatch.BackgroundServices;
using StatusWatch.Endpoints;
using StatusWatch.Helpers;
using StatusWatch.Models;
using StatusWatch.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

var connStr = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddScoped<IDbConnection>(_ => new NpgsqlConnection(connStr));
builder.Services.AddScoped<ServiceService>();
builder.Services.AddScoped<IncidentService>();
builder.Services.AddScoped<IncidentUpdateService>();
builder.Services.AddScoped<PingService>();
builder.Services.AddScoped<UserService>();

builder.Services.AddHttpClient();
builder.Services.AddHostedService<PingBackgroundService>();

builder.Services.AddOpenApi();

builder.Services
    .AddAuthentication("CookieAuth")
    .AddCookie("CookieAuth", options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
    });
builder.Services.AddAuthorization();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.MapOpenApi();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/openapi/v1.json", "StatusWatch API"));
}
else
{
    app.UseExceptionHandler("/Errors/500");
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Errors/{0}");

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapStatusEndpoints();

app.Run();
