using Newtonsoft.Json;
using Polly;
using PollyCircuitBreaker.Models;
using System.Net.Http;

namespace PollyCircuitBreaker.Services;

public class ProductService : IProductService
{
    internal readonly HttpClient _httpClient;
    internal readonly IAsyncPolicy _policy;

    public ProductService(IHttpClientFactory httpClientFactory, IAsyncPolicy policy)
    {
        _httpClient = httpClientFactory.CreateClient("FakeStoreClient");
        _policy = policy;
    }

    public async Task<ProductModel> GetProductByIdAsync(int id, CancellationToken cs = default)
    {
        try
        {
            HttpResponseMessage response = await _policy.ExecuteAsync(
                () => _httpClient.GetAsync($"/products/{id}", cs)
            );
            response.EnsureSuccessStatusCode();
            string jsonStr = await response.Content.ReadAsStringAsync(cs);

            var model = JsonConvert.DeserializeObject<ProductModel>(jsonStr);
            return model!;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
