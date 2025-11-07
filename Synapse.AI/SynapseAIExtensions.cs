using Microsoft.Extensions.DependencyInjection;
using Synapse.AI.Services;
using Synapse.Core;
using Synapse.Core.Evolution;
using System;

namespace Synapse.AI
{
    /// <summary>
    /// Synapse AI 扩展方法
    /// </summary>
    public static class SynapseAIExtensions
    {
        /// <summary>
        /// 添加 Synapse AI 功能
        /// </summary>
        public static IServiceCollection AddSynapseAI(
            this IServiceCollection services,
            Action<AIOptions>? configure = null)
        {
            var options = new AIOptions();
            configure?.Invoke(options);
            
            services.AddSingleton(options);
            
            // 注册源代码提取器（如果启用）
            if (options.IncludeSourceCode && !string.IsNullOrEmpty(options.SourceCodePath))
            {
                services.AddSingleton(sp => 
                    new SourceCodeExtractor(
                        options.SourceCodePath, 
                        sp.GetRequiredService<Microsoft.Extensions.Logging.ILogger<SourceCodeExtractor>>()));
            }
            
            services.AddSingleton<ICodeGenerator, GeminiCodeGenerator>();
            services.AddSingleton<ICodeCompiler, RoslynCodeCompiler>();
            services.AddSingleton<IAIEvolutionEngine, AIEvolutionEngine>();
            
            Console.WriteLine("✅ Synapse AI 引擎已启用");
            if (options.IncludeSourceCode)
            {
                Console.WriteLine("✅ 源代码分析已启用");
            }
            
            return services;
        }
    }
    
    /// <summary>
    /// AI 配置选项
    /// </summary>
    public class AIOptions
    {
        public string Provider { get; set; } = "Gemini";
        public string? ApiKey { get; set; }
        public string? Endpoint { get; set; }
        public string Model { get; set; } = "gemini-2.0-flash-exp";
        public int GenerationThreshold { get; set; } = 100;
        public bool AutoSwitch { get; set; } = true;
        public bool ForceGeneration { get; set; } = false;
        public bool IncludeSourceCode { get; set; } = false;
        public string? SourceCodePath { get; set; }
        
        /// <summary>
        /// 优化间隔时间（小时），防止同一个方法被重复优化。默认24小时
        /// </summary>
        public int OptimizationIntervalHours { get; set; } = 24;
    }
}
