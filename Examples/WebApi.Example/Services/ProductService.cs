using Synapse.Core.Attributes;

namespace WebApi.Example
{
    public class ProductService : IProductService
    {
        // 默认实现 - 性能较差
        [Evolvable]
        [Gene("DEFAULT")]
        public async Task<List<Product>> SearchProducts(string query)
        {
            // 模拟慢速查询
            await Task.Delay(100);
            
            var allProducts = GetAllProducts();
            return allProducts.Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
        }
        
        // 优化版本 1 - 使用并行查询
        [Gene("OPTIMIZED_V1")]
        public async Task<List<Product>> SearchProducts_V1(string query)
        {
            await Task.Delay(30);
            
            var allProducts = GetAllProducts();
            return allProducts.AsParallel()
                .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        
        // 优化版本 2 - 使用缓存
        [Gene("OPTIMIZED_V2")]
        public async Task<List<Product>> SearchProducts_V2(string query)
        {
            await Task.Delay(10);
            
            // 模拟从缓存读取
            var allProducts = GetAllProducts();
            return allProducts
                .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
        
        private List<Product> GetAllProducts()
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "iPhone 15 Pro", Price = 999.99m },
                new Product { Id = 2, Name = "MacBook Pro", Price = 2499.99m },
                new Product { Id = 3, Name = "iPad Pro", Price = 799.99m },
                new Product { Id = 4, Name = "AirPods Pro", Price = 249.99m },
                new Product { Id = 5, Name = "Apple Watch", Price = 399.99m },
                new Product { Id = 6, Name = "Mac Pro", Price = 5999.99m }
            };
        }
    }
}
