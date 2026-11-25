// ReSharper disable StringLiteralTypo
var builder = DistributedApplication.CreateBuilder(args);

builder.AddDotnetProject("apiserver", "../Template.ApiServer.Host/Template.ApiServer.Host.csproj")
    .WithHttpHealthCheck("/health");

builder.Build().Run();
