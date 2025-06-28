using Projects;

var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Company_Platform_Domain_App>("domain");

await builder
    .Build()
    .RunAsync();