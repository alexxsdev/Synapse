using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Synapse.Core.Services;

namespace Synapse.Core.Evolution
{
    /// <summary>
    /// 进化引擎接口
    /// </summary>
    public interface IEvolutionEngine
    {
        Task StartAsync(CancellationToken cancellationToken = default);
        Task StopAsync(CancellationToken cancellationToken = default);
        void AnalyzePerformance();
    }
    
    /// <summary>
    /// 进化引擎 - 自动分析性能并切换基因
    /// </summary>
    public class EvolutionEngine : IEvolutionEngine
    {
        private readonly SynapseOptions _options;
        private readonly IMetricsCollector _metrics;
        private readonly ILogger<EvolutionEngine> _logger;
        private Timer? _analysisTimer;
        
        public EvolutionEngine(
            SynapseOptions options,
            IMetricsCollector metrics,
            ILogger<EvolutionEngine> logger)
        {
            _options = options;
            _metrics = metrics;
            _logger = logger;
        }
        
        public Task StartAsync(CancellationToken cancellationToken = default)
        {
            if (!_options.Enabled || !_options.AutoEvolution)
            {
                _logger.LogInformation("Synapse 自动进化未启用");
                return Task.CompletedTask;
            }
            
            _logger.LogInformation("🧬 Synapse 进化引擎已启动");
            
            // 每30秒分析一次性能
            _analysisTimer = new Timer(
                _ => AnalyzePerformance(),
                null,
                TimeSpan.FromSeconds(30),
                TimeSpan.FromSeconds(30));
            
            return Task.CompletedTask;
        }
        
        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            _analysisTimer?.Dispose();
            _logger.LogInformation("Synapse 进化引擎已停止");
            return Task.CompletedTask;
        }
        
        public void AnalyzePerformance()
        {
            try
            {
                var methodGroups = _metrics.GetAllMetrics().GroupBy(m => m.MethodName);
                
                foreach (var group in methodGroups)
                {
                    AnalyzeMethod(group.Key, group.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "性能分析失败");
            }
        }
        
        private void AnalyzeMethod(string methodName, System.Collections.Generic.List<GeneMetrics> geneMetrics)
        {
            // 输出所有基因的性能指标（即使只有一个基因也输出）
            foreach (var gene in geneMetrics.Where(g => g.TotalExecutions > 0))
            {
                _logger.LogInformation(
                    "📊 [指标] {MethodName} - {GeneId}\n" +
                    "   • 平均时间: {Avg:F2}ms\n" +
                    "   • P95: {P95:F2}ms\n" +
                    "   • P99: {P99:F2}ms\n" +
                    "   • 执行次数: {Count}\n" +
                    "   • 成功率: {SuccessRate:F1}%",
                    methodName, gene.GeneId, gene.AverageTime, gene.P95, gene.P99, 
                    gene.TotalExecutions, gene.SuccessRate);
            }
            
            // 过滤有效基因（达到最小样本数的基因）
            var validGenes = geneMetrics
                .Where(g => g.TotalExecutions >= _options.MinSampleSize)
                .ToList();
            
            if (validGenes.Count == 0)
            {
                _logger.LogInformation("⏳ 方法 {MethodName} 样本数不足（需要 {MinSamples} 次），当前最高: {Current} 次", 
                    methodName, _options.MinSampleSize, geneMetrics.Max(g => g.TotalExecutions));
                return;
            }
            
            // 找到最优基因
            var bestGene = validGenes.OrderBy(g => g.P95).First();
            
            _logger.LogInformation(
                "🏆 方法 {MethodName} 最优基因: {GeneId} (P95: {P95}ms)",
                methodName, bestGene.GeneId, bestGene.P95);
            
            // 检查是否需要优化
            if (bestGene.P95 > _options.PerformanceThreshold)
            {
                _logger.LogWarning(
                    "⚠️  方法 {MethodName} 性能不佳 (P95: {P95}ms > {Threshold}ms)",
                    methodName, bestGene.P95, _options.PerformanceThreshold);
                
                // 触发 AI 优化（如果启用）
                if (_options.EnableAI)
                {
                    _logger.LogInformation("🤖 触发 AI 代码生成...");
                    // AI 生成逻辑在 Synapse.AI 包中实现
                }
            }
        }
    }
}
