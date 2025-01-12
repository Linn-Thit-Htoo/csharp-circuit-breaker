using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PollyCircuitBreaker.Services;

namespace PollyCircuitBreaker.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    internal readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id, CancellationToken cs)
    {
        try
        {
            var result = await _productService.GetProductByIdAsync(id, cs);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}
