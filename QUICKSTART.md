# 🚀 Synapse 快速开始指南

5 分钟内让你的代码开始自我进化！

## 前置要求

- .NET 8.0 SDK
- Gemini API Key（免费获取：https://makersuite.google.com/app/apikey）

## 步骤 1：创建项目

```bash
dotnet new webapi -n MyEvolvingApi
cd MyEvolvingApi
```

## 步骤 2：添加 Synapse 包

```bash
dotnet add package Synapse.Core
dotnet add package Synapse.AI
```

## 步骤 3：配置 appsettings.json

```json
{
  "Synapse": {
    "Enabled": true,
    "EnableAI": true,
    "AutoEvolution": true,
    "AI": {
      "ApiKey": "YOUR_GEMINI_API_KEY_HERE",
      "GenerationThreshold": 20
    }
  }
}
```

## 步骤 4：配置 Program.cs

```csharp
using Synapse.Core;
using Synapse.AI;

var builder = WebApplication.CreateBuilder(args);

// 添加 Synapse
builder.Services.AddSynapse(builder.Configuration);
builder.Services.AddSynapseAI(options =>
{
    builder.Configuration.GetSection("Synapse:AI").Bind(options);
});

builder.Services.AddControllers();
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();
app.MapControllers();
app.Run();
```

## 步骤 5：创建可进化的服务

创建 `ProductService.cs`：

```csharp
using Synapse.Core.Attributes;

public interface IProductService
{
    Task<List<Product>> SearchProducts(string query);
}

public class ProductService : IProductService
{
    [Evolvable]
    [Gene("DEFAULT")]
    public async Task<List<Product>> SearchProducts(string query)
    {
        // 初始实现 - 性能较差
        await Task.Delay(100);
        
        var products = GetAllProducts();
        return products
            .Where(p => p.Name.Contains(query))
            .ToList();
    }
    
    private List<Product> GetAllProducts()
    {
        return new List<Product>
        {
            new Product { Id = 1, Name = "iPhone 15 Pro", Price = 999 },
            new Product { Id = 2, Name = "MacBook Pro", Price = 2499 },
            new Product { Id = 3, Name = "iPad Pro", Price = 799 }
        };
    }
}

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}
```

## 步骤 6：创建 API 控制器

创建 `ProductController.cs`：

```csharp
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IProductService _service;
    
    public ProductController(IProductService service)
    {
        _service = service;
    }
    
    [HttpGet("search")]
    public async Task<ActionResult<List<Product>>> Search([FromQuery] string query = "Pro")
    {
        var products = await _service.SearchProducts(query);
        return Ok(products);
    }
}
```

## 步骤 7：运行并测试

```bash
dotnet run
```

在另一个终端，多次调用 API（至少 20 次）：

```bash
# Linux/Mac
for i in {1..25}; do curl http://localhost:5000/api/product/search?query=Pro; done

# Windows PowerShell
1..25 | ForEach-Object { Invoke-WebRequest http://localhost:5000/api/product/search?query=Pro }
```

## 步骤 8：观察进化

查看控制台输出，你会看到：

```
📊 [指标] SearchProducts - DEFAULT
   • 平均时间: 105.23ms
   • P95: 120.45ms
   • 执行次数: 25

🧬 [进化引擎] 性能仍有优化空间，触发 AI 代码生成...
🤖 [AI 引擎] 正在生成优化代码...
✅ [编译器] 编译成功: AI_OPTIMIZED_20241107120000
🎉 新基因已就绪！
```

## 步骤 9：查看生成的代码

```bash
cat .synapse/genes/AI_OPTIMIZED_20241107120000.cs
```

你会看到 AI 生成的优化版本，可能包含：
- 并行处理
- 缓存优化
- 算法改进
- 性能提示

## 步骤 10：验证性能提升

继续调用 API，观察新基因的性能：

```
📊 [指标] SearchProducts - AI_OPTIMIZED_20241107120000
   • 平均时间: 35.67ms  ⬇️ 降低 66%
   • P95: 42.12ms        ⬇️ 降低 65%
   • 执行次数: 50
```

## 🎉 完成！

你的代码现在会自动进化了！

## 下一步

### 1. 添加更多可进化方法

```csharp
[Evolvable]
public async Task<Order> ProcessOrder(OrderRequest request)
{
    // 你的实现
}

[Evolvable]
public List<ReportData> GenerateReport(DateTime start, DateTime end)
{
    // 你的实现
}
```

### 2. 自定义配置

```json
{
  "Synapse": {
    "PerformanceThreshold": 30.0,  // 降低阈值，更积极优化
    "AI": {
      "GenerationThreshold": 10,   // 更早触发 AI
      "AutoSwitch": true           // 自动切换到新基因
    }
  }
}
```

### 3. 手动提供优化版本

```csharp
[Evolvable]
[Gene("DEFAULT")]
public async Task<List<Product>> SearchProducts(string query)
{
    // 默认实现
}

[Gene("OPTIMIZED")]
public async Task<List<Product>> SearchProducts_Optimized(string query)
{
    // 你的优化版本
}
```

然后在配置中切换：

```json
{
  "Synapse": {
    "Genes": {
      "SearchProducts": "OPTIMIZED"
    }
  }
}
```

### 4. 监控和诊断

查看所有生成的基因：

```bash
ls -la .synapse/genes/
```

查看基因元数据：

```bash
cat .synapse/genes/AI_OPTIMIZED_20241107120000.json
```

### 5. 集成到 CI/CD

```yaml
# .github/workflows/deploy.yml
- name: Copy evolved genes
  run: |
    mkdir -p $DEPLOY_PATH/.synapse/genes
    cp -r .synapse/genes/* $DEPLOY_PATH/.synapse/genes/
```

## 常见问题

### Q: AI 不生成代码？

A: 检查：
1. API Key 是否正确
2. 执行次数是否达到阈值（默认 20 次）
3. 性能是否低于阈值（默认 50ms）
4. 日志中是否有错误

### Q: 编译失败？

A: 查看 `.synapse/genes/*.json` 中的错误信息，AI 生成的代码可能需要调整。

### Q: 如何禁用 AI？

A: 设置 `"EnableAI": false` 或移除 `AddSynapseAI()` 调用。

### Q: 如何回滚到旧版本？

A: 在配置中指定基因 ID：

```json
{
  "Synapse": {
    "Genes": {
      "SearchProducts": "DEFAULT"
    }
  }
}
```

## 获取帮助

- 查看完整文档：[README.md](README.md)
- 查看示例项目：[Examples/WebApi.Example](Examples/WebApi.Example)
- 报告问题：GitHub Issues

---

**开始让你的代码自我进化吧！** 🚀
