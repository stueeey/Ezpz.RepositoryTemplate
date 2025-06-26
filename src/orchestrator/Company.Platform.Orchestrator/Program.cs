using Aspire.Hosting;
using Projects;

var builder = DistributedApplication.CreateBuilder(args);

///builder.AddProject("mainapp")
//    .WithHttpsHealthCheck();

await builder
    .Build()
    .RunAsync();