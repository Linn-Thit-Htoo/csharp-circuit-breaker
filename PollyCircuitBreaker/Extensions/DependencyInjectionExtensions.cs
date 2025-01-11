using Polly;
using PollyCircuitBreaker.Services;

namespace PollyCircuitBreaker.Extensions
{
    public static class DependencyInjectionExtensions
    {
        public static IServiceCollection AddDependencies(this IServiceCollection services, WebApplicationBuilder builder)
        {
            builder.Services.AddHttpClient("FakeStoreClient", opt =>
            {
                opt.BaseAddress = new Uri("https://fakestoreapis.com");
            });

            var circuitBreakerPolicy = Policy
            .Handle<HttpRequestException>()
            .CircuitBreakerAsync(
                exceptionsAllowedBeforeBreaking: 2,
                durationOfBreak: TimeSpan.FromMinutes(1),
                onBreak: (exception, timespan) =>
                {
                    Console.WriteLine($"Circuit broken due to: {exception.Message}");
                },
                onReset: () => Console.WriteLine("Circuit closed."),
                onHalfOpen: () => Console.WriteLine("Circuit in half-open state.")
            );

            builder.Services.AddSingleton<IAsyncPolicy>(circuitBreakerPolicy);
            builder.Services.AddScoped<IProductService, ProductService>();

            return services;
        }
    }
}
