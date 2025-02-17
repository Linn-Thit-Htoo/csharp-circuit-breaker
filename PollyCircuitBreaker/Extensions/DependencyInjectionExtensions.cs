using Polly;
using PollyCircuitBreaker.Services;

namespace PollyCircuitBreaker.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDependencies(
        this IServiceCollection services,
        WebApplicationBuilder builder,
        ILogger logger
    )
    {
        builder.Services.AddHttpClient(
            "FakeStoreClient",
            opt =>
            {
                opt.BaseAddress = new Uri("https://fakestoreapis.com");
            }
        );

        var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 2,
                durationOfBreak: TimeSpan.FromSeconds(10),
                onBreak: (exception, timespan) =>
                {
                    logger.LogError($"Circuit broken due to: {exception.Message}");
                },
                onReset: () => logger.LogError("Circuit closed."),
                onHalfOpen: () => logger.LogError("Circuit in half-open state.")
            );

        builder.Services.AddSingleton<IAsyncPolicy>(circuitBreakerPolicy);
        builder.Services.AddScoped<IProductService, ProductService>();

        return services;
    }
}
