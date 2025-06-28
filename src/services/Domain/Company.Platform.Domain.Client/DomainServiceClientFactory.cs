using Company.Platform.Domain.App;
using Grpc.Net.Client;
using Microsoft.Extensions.DependencyInjection;

namespace Company.Platform.Domain.Client;

public static class DomainServiceClientFactory
{
    public static IServiceCollection AddDomainServiceClient(this IServiceCollection services, Uri baseAddress)
    {
        services.AddGrpcClient<Greeter.GreeterClient>(o => { o.Address = baseAddress; });

        return services;
    }

    public static Greeter.GreeterClient CreateGreeterClient(string baseAddress)
    {
        var channel = GrpcChannel.ForAddress(baseAddress);
        return new Greeter.GreeterClient(channel);
    }
}