# Synapse WebAPI 示例

这是一个完整的 ASP.NET Core Web API 示例，展示了 Synapse 自进化框架的使用。

## 快速开始

### 1. 配置 API Key

编辑 `appsettings.json`，设置你的 Gemini API Key：

```json
{
  "Synapse": {
    "AI": {
      "ApiKey": "your-gemini-api-key-here"
    }
  }
}
```

### 2. 运行项目

```bash
cd Synapse.Framework/Examples/WebApi.Example
dotnet run
```

### 3. 测试 API

访问 Swagger UI：
```
https://localhost:5001/swagger
```

或直接测试：
```bash
curl https://localhost:5001/api/product/search?query=Pro
```

## 功能演示

### 自动性能监控

框架会自动监控 `SearchProducts` 方法的性能：
- 执行时间
- P95/P99 延迟
- 成功率

### 自动代码进化

当满足以下条件时，AI 会自动生成优化代码：
1. 执行次数 >= 20 次（可配置）
2. P95 延迟 > 50ms（可配置）

### 查看进化过程

查看日志输出：
```
🧬 [进化引擎] 开始分析...
📊 [指标] DEFAULT: P95=105ms, 执行次数=25
🤖 [AI 引擎] 正在生成优化代码...
✅ [编译器] 编译成功: AI_OPTIMIZED_20241107120000
🎉 新基因已就绪！
```

### 查看缓存的基因

生成的代码会保存在：
```
.synapse/genes/
├── AI_OPTIMIZED_20241107120000.dll
├── AI_OPTIMIZED_20241107120000.cs
└── AI_OPTIMIZED_20241107120000.json
```

## 配置说明

### Synapse 配置

```json
{
  "Synapse": {
    "Enabled": true,              // 启用框架
    "EnableAI": true,             // 启用 AI 功能
    "AutoEvolution": true,        // 自动进化
    "PerformanceThreshold": 50.0, // 性能阈值（ms）
    "MinSampleSize": 10,          // 最小样本数
    "CacheDirectory": ".synapse/genes"
  }
}
```

### AI 配置

```json
{
  "AI": {
    "Provider": "Gemini",
    "ApiKey": "your-api-key",
    "GenerationThreshold": 20,    // 触发 AI 生成的最小执行次数
    "AutoSwitch": true,           // 自动切换到新基因
    "ForceGeneration": false      // 强制生成（忽略性能阈值）
  }
}
```

## 测试场景

### 场景 1：触发自动优化

1. 多次调用 API（至少 20 次）
2. 观察日志，等待 AI 生成新代码
3. 新代码会自动编译并加载

### 场景 2：手动切换基因

修改配置文件，切换到不同的基因实现：
```json
{
  "Synapse": {
    "Genes": {
      "SearchProducts": "OPTIMIZED_V1"  // 或 OPTIMIZED_V2, AI_OPTIMIZED_xxx
    }
  }
}
```

### 场景 3：性能对比

使用压测工具对比不同基因的性能：
```bash
# 使用 Apache Bench
ab -n 1000 -c 10 https://localhost:5001/api/product/search?query=Pro
```

## 故障排查

### AI 不生成代码

检查：
1. API Key 是否正确
2. 执行次数是否达到阈值
3. 性能是否低于阈值
4. 日志中是否有错误信息

### 编译失败

查看 `.synapse/genes/*.json` 中的错误信息

### 基因未加载

检查：
1. DLL 文件是否存在
2. 方法签名是否匹配
3. 日志中的加载信息

## 下一步

- 添加更多 Evolvable 方法
- 自定义 AI Prompt
- 实现自定义指标收集
- 集成到生产环境
