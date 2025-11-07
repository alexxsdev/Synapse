using Microsoft.AspNetCore.Mvc;
using Synapse.Core.Attributes;

namespace WebApi.Example.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        
        public ProductController(IProductService productService)
        {
            _productService = productService;
        }
        
        /// <summary>
        /// 搜索产品 - 这个方法会自动进化优化
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<List<Product>>> SearchProducts([FromQuery] string query = "Pro")
        {
            var products = await _productService.SearchProducts(query);
            return Ok(products);
        }
        
        /// <summary>
        /// 获取所有产品
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            // 模拟获取所有产品
            await Task.Delay(50);
            
            return Ok(new List<Product>
            {
                new Product { Id = 1, Name = "iPhone 15 Pro", Price = 999.99m },
                new Product { Id = 2, Name = "MacBook Pro", Price = 2499.99m },
                new Product { Id = 3, Name = "iPad Pro", Price = 799.99m },
                new Product { Id = 4, Name = "AirPods Pro", Price = 249.99m }
            });
        }
    }
}
