using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Synapse.Core.Evolution;

namespace Synapse.Core.Services
{
    /// <summary>
    /// Synapse 后台服务 - 自动启动进化引擎和 AI 进化引擎
    /// </summary>
    public class SynapseHostedService : IHostedService
    {
        private readonly IEvolutionEngine _evolutionEngine;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SynapseHostedService> _logger;
        private Timer? _aiAnalysisTimer;

        public SynapseHostedService(
            IEvolutionEngine evolutionEngine,
            IServiceProvider serviceProvider,
            ILogger<SynapseHostedService> logger)
        {
            _evolutionEngine = evolutionEngine;
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🚀 Synapse 框架正在启动...");
            await _evolutionEngine.StartAsync(cancellationToken);
            
            // 尝试启动 AI 进化引擎（如果已注册）
            var aiEngine = _serviceProvider.GetService<IAIEvolutionEngine>();
            if (aiEngine != null)
            {
                _logger.LogInformation("🤖 AI 进化引擎已启用");
                
                // 每30秒运行一次 AI 分析
                _aiAnalysisTimer = new Timer(
                    async _ => await RunAIAnalysisAsync(),
                    null,
                    TimeSpan.FromSeconds(30),
                    TimeSpan.FromSeconds(30));
            }
        }

        private async Task RunAIAnalysisAsync()
        {
            try
            {
                var aiEngine = _serviceProvider.GetService<IAIEvolutionEngine>();
                if (aiEngine != null)
                {
                    await aiEngine.AnalyzeAndEvolveAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AI 分析执行失败");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("🛑 Synapse 框架正在停止...");
            _aiAnalysisTimer?.Dispose();
            await _evolutionEngine.StopAsync(cancellationToken);
        }
    }
}

