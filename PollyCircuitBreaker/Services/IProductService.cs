using PollyCircuitBreaker.Models;

namespace PollyCircuitBreaker.Services;

public interface IProductService
{
    Task<ProductModel> GetProductByIdAsync(int id, CancellationToken cs = default);
}
