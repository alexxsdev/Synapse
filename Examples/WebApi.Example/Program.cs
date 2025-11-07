using Synapse.Core;
using Synapse.Core.Attributes;

var builder = WebApplication.CreateBuilder(args);

// 添加 Synapse 自进化框架
builder.Services.AddSynapse(options =>
{
    options.EnableAI = true;
    options.GeminiApiKey = builder.Configuration["Synapse:AI:ApiKey"];
    options.AutoEvolution = true;
    options.PerformanceThreshold = 50.0; // 50ms
});

// 添加控制器
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 注册服务
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("🧬 Synapse 自进化框架已启动");
Console.WriteLine("📊 访问 /swagger 查看 API 文档");

app.Run();

// ==================== 服务示例 ====================

public interface IProductService
{
    Task<List<Product>> SearchProducts(string query);
}

public class ProductService : IProductService
{
    /// <summary>
    /// 搜索产品 - 可进化方法
    /// </summary>
    [Evolvable(Goal = "降低搜索延迟", PerformanceThreshold = 50.0)]
    public async Task<List<Product>> SearchProducts(string query)
    {
        // 默认使用简单搜索
        return await SearchSimple(query);
    }
    
    [Gene("SIMPLE", Description = "简单的 LINQ 搜索", IsDefault = true)]
    private async Task<List<Product>> SearchSimple(string query)
    {
        // 模拟数据库查询
        await Task.Delay(100);
        
        var products = GetAllProducts();
        return products
            .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
    
    [Gene("OPTIMIZED", Description = "优化的并行搜索")]
    private async Task<List<Product>> SearchOptimized(string query)
    {
        // 模拟优化的查询
        await Task.Delay(30);
        
        var products = GetAllProducts();
        return products
            .AsParallel()
            .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
    
    [Gene("CACHED", Description = "带缓存的搜索")]
    private async Task<List<Product>> SearchCached(string query)
    {
        // 模拟缓存查询
        await Task.Delay(10);
        
        // 实际应用中这里会使用 Redis 等缓存
        var products = GetAllProducts();
        return products
            .Where(p => p.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
    
    private List<Product> GetAllProducts()
    {
        return new List<Product>
        {
            new Product { Id = 1, Name = "iPhone 15", Price = 999 },
            new Product { Id = 2, Name = "MacBook Pro", Price = 2499 },
            new Product { Id = 3, Name = "AirPods Pro", Price = 249 },
        };
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
