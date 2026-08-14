using Microsoft.Extensions.Hosting.WindowsServices;

using Template.ApiServer.Host.Application;

//--------------------------------------------------------------------------------
// Configure builder
//--------------------------------------------------------------------------------
Directory.SetCurrentDirectory(AppContext.BaseDirectory);
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = WindowsServiceHelpers.IsWindowsService() ? AppContext.BaseDirectory : default
});

// System
builder.ConfigureSystem();

// Host
builder.ConfigureHost();

// Logging
builder.ConfigureLogging();

// Http
builder.ConfigureHttp();
// API
builder.ConfigureApi();
// Authentication
builder.ConfigureAuthentication();
// Rate limit
builder.ConfigureRateLimiter();
// Compress
builder.ConfigureCompression();
// OpenApi
builder.ConfigureOpenApi();

// Health
builder.ConfigureHealth();
// Metrics
builder.ConfigureTelemetry();

// Components
builder.ConfigureComponents();

//--------------------------------------------------------------------------------
// Configure the HTTP request pipeline.
//--------------------------------------------------------------------------------
var app = builder.Build();

// Startup information
app.LogStartupInformation();

// Forwarded headers
app.UseForwardedHeaders();

// Error handler
app.UseErrorHandler();

// Compression
app.UseCompression();

// Logging
app.UseLogging();

// Rate limit
app.UseRateLimiter();

// Authentication
app.UseAuthentication();
app.UseAuthorization();

// Logging context
app.UseLoggingContext();

// End point
app.MapEndpoints();

// Initialize
await app.InitializeApplicationAsync();

// Run
await app.RunAsync();

[ExcludeFromCodeCoverage]
public partial class Program
{
}
