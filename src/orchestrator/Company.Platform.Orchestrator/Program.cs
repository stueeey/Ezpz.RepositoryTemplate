using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Company_Platform_Domain_App>("domain");
builder.AddProject<Company_Platform_UrlShortener_App>("urlshortener");

await builder
    .Build()
    .RunAsync();