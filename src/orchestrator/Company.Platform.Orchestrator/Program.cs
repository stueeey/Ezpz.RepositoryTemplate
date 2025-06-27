var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Company_Platform_Domain_App>("domain");

await builder
    .Build()
    .RunAsync();