using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Synapse.Core.Evolution;
using Synapse.Core.Services;

namespace Synapse.AI.Services
{
    /// <summary>
    /// AI 进化引擎实现 - 使用 Gemini 生成优化代码
    /// </summary>
    public class AIEvolutionEngine : IAIEvolutionEngine
    {
        private readonly AIOptions _aiOptions;
        private readonly IMetricsCollector _metrics;
        private readonly ICodeGenerator _codeGenerator;
        private readonly ICodeCompiler _compiler;
        private readonly IDynamicGeneLoader _geneLoader;
        private readonly IGeneCache _geneCache;
        private readonly ILogger<AIEvolutionEngine> _logger;
        private readonly SourceCodeExtractor? _sourceCodeExtractor;
        
        // 记录每个方法上次优化的时间
        private readonly System.Collections.Concurrent.ConcurrentDictionary<string, DateTime> _lastOptimizationTime = new();
        
        // 优化间隔（从配置读取）
        private readonly TimeSpan _optimizationInterval;
        
        public AIEvolutionEngine(
            AIOptions aiOptions,
            IMetricsCollector metrics,
            ICodeGenerator codeGenerator,
            ICodeCompiler compiler,
            IDynamicGeneLoader geneLoader,
            IGeneCache geneCache,
            ILogger<AIEvolutionEngine> logger,
            SourceCodeExtractor? sourceCodeExtractor = null)
        {
            _aiOptions = aiOptions;
            _metrics = metrics;
            _codeGenerator = codeGenerator;
            _compiler = compiler;
            _geneLoader = geneLoader;
            _geneCache = geneCache;
            _logger = logger;
            _sourceCodeExtractor = sourceCodeExtractor;
            
            // 从配置读取优化间隔时间
            _optimizationInterval = TimeSpan.FromHours(_aiOptions.OptimizationIntervalHours);
            
            _geneCache.LoadAllCachedGenes(_geneLoader);
            
            _logger.LogInformation("🕐 AI 优化间隔: {Hours} 小时", _aiOptions.OptimizationIntervalHours);
        }
        
        public async Task AnalyzeAndEvolveAsync()
        {
            try
            {
                _logger.LogInformation("🤖 [AI] 开始分析...");
                
                var methodGroups = _metrics.GetAllMetrics().GroupBy(m => m.MethodName);
                
                foreach (var group in methodGroups)
                {
                    await AnalyzeMethodAsync(group.Key, group.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI 分析失败");
            }
        }
        
        private async Task AnalyzeMethodAsync(string methodName, System.Collections.Generic.List<GeneMetrics> geneMetrics)
        {
            var totalExecutions = geneMetrics.Sum(g => g.TotalExecutions);
            
            if (totalExecutions < _aiOptions.GenerationThreshold)
            {
                return;
            }
            
            var bestGene = geneMetrics.OrderBy(g => g.P95).First();
            
            _logger.LogInformation("当前最优: {GeneId} (P95: {P95}ms)", bestGene.GeneId, bestGene.P95);
            
            if (!_aiOptions.ForceGeneration && bestGene.P95 <= 50.0)
            {
                return;
            }
            
            // 🕐 检查优化频率限制（一天只优化一次）
            if (_lastOptimizationTime.TryGetValue(methodName, out var lastTime))
            {
                var timeSinceLastOptimization = DateTime.UtcNow - lastTime;
                if (timeSinceLastOptimization < _optimizationInterval)
                {
                    var remainingTime = _optimizationInterval - timeSinceLastOptimization;
                    _logger.LogInformation("⏳ 方法 {MethodName} 距离上次优化仅 {Hours:F1} 小时，需等待 {RemainingHours:F1} 小时后才能再次优化", 
                        methodName, 
                        timeSinceLastOptimization.TotalHours, 
                        remainingTime.TotalHours);
                    return;
                }
            }
            
            _logger.LogInformation("触发 AI 生成...");
            
            await GenerateNewGeneAsync(methodName, bestGene);
            
            // 记录本次优化时间
            _lastOptimizationTime[methodName] = DateTime.UtcNow;
        }
        
        private async Task GenerateNewGeneAsync(string methodName, GeneMetrics currentBest)
        {
            try
            {
                // 尝试提取真实的源代码
                string? sourceCode = null;
                string? classContext = null;
                
                if (_sourceCodeExtractor != null)
                {
                    _logger.LogInformation("🔍 正在提取方法源代码...");
                    sourceCode = _sourceCodeExtractor.ExtractMethodSource(methodName);
                    
                    if (!string.IsNullOrEmpty(sourceCode))
                    {
                        _logger.LogInformation("✅ 已提取源代码 ({Length} 字符)", sourceCode.Length);
                        // 同时提取类的上下文信息
                        classContext = _sourceCodeExtractor.ExtractClassContext(methodName);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ 未能提取源代码，将使用通用分析");
                    }
                }
                
                // 构建针对实际方法的性能分析报告
                var analysisPrompt = $@"
方法名称: {methodName}
当前基因: {currentBest.GeneId}

性能指标:
• 平均响应时间: {currentBest.AverageTime:F2}ms
• P95 延迟: {currentBest.P95:F2}ms  
• P99 延迟: {currentBest.P99:F2}ms
• 最小时间: {currentBest.MinTime:F2}ms
• 最大时间: {currentBest.MaxTime:F2}ms
• 执行次数: {currentBest.TotalExecutions}
• 成功率: {currentBest.SuccessRate:F1}%

{(classContext != null ? $@"
类上下文:
```csharp
{classContext}
```

" : "")}

{(sourceCode != null ? $@"
当前源代码:
```csharp
{sourceCode}
```

请基于上述真实的源代码，分析性能瓶颈并提供优化后的完整方法代码。
" : @"
请分析这个方法可能的性能瓶颈，并提供具体的优化建议。
")}

优化重点：
1. 数据库查询优化（索引、N+1问题、AsNoTracking）
2. 并发处理优化（Task.WhenAll、SemaphoreSlim）
3. 缓存策略（MemoryCache、分布式缓存）
4. 异步/并行操作优化（ConfigureAwait、ValueTask）
5. 资源管理优化（连接池、对象池）

要求：
- 保持方法签名不变
- 保持方法的业务逻辑不变
- 只优化性能相关的代码
- 提供完整的可编译的方法代码
- ⚠️ 不要包含热重载检查代码（_hotReloadService 相关代码）
- ⚠️ 直接从方法的实际业务逻辑开始（跳过热重载部分）
";
                
                _logger.LogInformation("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                _logger.LogInformation("🤖 [AI] 正在分析方法: {MethodName}", methodName);
                _logger.LogInformation("📊 性能数据: P95={P95}ms, 平均={Avg}ms, 执行={Count}次", 
                    currentBest.P95, currentBest.AverageTime, currentBest.TotalExecutions);
                
                var optimizationSuggestion = await _codeGenerator.GenerateOptimizedCodeAsync(
                    analysisPrompt,
                    currentBest,
                    $"针对 {methodName} 提供优化建议");
                
                if (string.IsNullOrEmpty(optimizationSuggestion))
                {
                    _logger.LogWarning("❌ AI 未生成优化建议");
                    return;
                }
                
                _logger.LogInformation("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                _logger.LogInformation("💡 AI 优化建议:\n{Suggestion}", optimizationSuggestion);
                _logger.LogInformation("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
                
                // 保存优化建议到文件（包含元数据用于热重载）
                try
                {
                    var suggestionsDir = ".synapse/suggestions";
                    Directory.CreateDirectory(suggestionsDir);
                    
                    var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                    var suggestionFile = Path.Combine(suggestionsDir, $"{methodName}_{timestamp}.txt");
                    
                    // 保存为 JSON 格式，方便前端解析
                    var metadata = new
                    {
                        methodName,
                        geneId = currentBest.GeneId,
                        timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        metrics = new
                        {
                            averageTime = currentBest.AverageTime,
                            p95 = currentBest.P95,
                            p99 = currentBest.P99,
                            minTime = currentBest.MinTime,
                            maxTime = currentBest.MaxTime,
                            executions = currentBest.TotalExecutions,
                            successRate = currentBest.SuccessRate
                        },
                        originalSourceCode = sourceCode,  // 保存原始源代码
                        hasSourceCode = !string.IsNullOrEmpty(sourceCode),
                        suggestion = optimizationSuggestion,
                        prompt = analysisPrompt,
                        status = "pending" // pending, applied, rejected
                    };
                    
                    var jsonContent = System.Text.Json.JsonSerializer.Serialize(metadata, new System.Text.Json.JsonSerializerOptions 
                    { 
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    });
                    
                    File.WriteAllText(suggestionFile.Replace(".txt", ".json"), jsonContent);
                    
                    // 同时保存文本版本用于查看
                    var reportContent = $@"━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
Synapse AI 性能优化报告
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━

{analysisPrompt}

AI 优化建议:
{optimizationSuggestion}

生成时间: {DateTime.Now:yyyy-MM-dd HH:mm:ss}
━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━
";
                    
                    File.WriteAllText(suggestionFile, reportContent);
                    _logger.LogInformation("📁 优化报告已保存: {File}", Path.GetFullPath(suggestionFile));
                }
                catch (Exception fileEx)
                {
                    _logger.LogWarning(fileEx, "保存优化报告失败");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "生成优化建议失败");
            }
        }
        
        private string GetCurrentCode(string geneId)
        {
            // 返回空字符串，让 AI 根据方法签名和性能指标直接生成优化代码
            // 实际项目中，这里应该通过反射或源代码分析获取真实的方法代码
            return string.Empty;
        }
    }
}
