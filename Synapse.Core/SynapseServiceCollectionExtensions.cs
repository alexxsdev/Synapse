using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Synapse.Core.Services;
using Synapse.Core.Telemetry;
using Synapse.Core.Evolution;
using System;

namespace Synapse.Core
{
    /// <summary>
    /// Synapse 服务注册扩展
    /// </summary>
    public static class SynapseServiceCollectionExtensions
    {
        /// <summary>
        /// 添加 Synapse 自进化框架
        /// </summary>
        public static IServiceCollection AddSynapse(
            this IServiceCollection services,
            Action<SynapseOptions>? configure = null)
        {
            var options = new SynapseOptions();
            configure?.Invoke(options);
            
            // 注册选项
            services.AddSingleton(options);
            
            // 注册核心服务
            services.AddSingleton<IMetricsCollector, MetricsCollector>();
            services.AddSingleton<Evolution.IGeneCache, GeneCache>();
            services.AddSingleton<Evolution.IDynamicGeneLoader, DynamicGeneLoader>();
            
            // 注册进化引擎
            services.AddSingleton<Evolution.IEvolutionEngine, EvolutionEngine>();
            
            // 注册后台服务（自动启动进化引擎）
            services.AddHostedService<Services.SynapseHostedService>();
            
            // 如果启用 AI，注册 AI 服务
            if (options.EnableAI)
            {
                // AI 服务在 Synapse.AI 包中
                // 这里只是占位，实际由 AddSynapseAI 扩展方法注册
            }
            
            Console.WriteLine("✅ Synapse 自进化框架已启用");
            
            return services;
        }
        
        /// <summary>
        /// 添加 Synapse 自进化框架（使用配置）
        /// </summary>
        public static IServiceCollection AddSynapse(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            return services.AddSynapse(options =>
            {
                configuration.GetSection("Synapse").Bind(options);
            });
        }
    }
    
    /// <summary>
    /// Synapse 配置选项
    /// </summary>
    public class SynapseOptions
    {
        /// <summary>
        /// 是否启用框架
        /// </summary>
        public bool Enabled { get; set; } = true;
        
        /// <summary>
        /// 是否启用 AI 功能
        /// </summary>
        public bool EnableAI { get; set; } = false;
        
        /// <summary>
        /// 是否自动进化
        /// </summary>
        public bool AutoEvolution { get; set; } = true;
        
        /// <summary>
        /// 性能阈值（毫秒）
        /// </summary>
        public double PerformanceThreshold { get; set; } = 100.0;
        
        /// <summary>
        /// 最小采样数
        /// </summary>
        public int MinSampleSize { get; set; } = 100;
        
        /// <summary>
        /// 基因缓存目录
        /// </summary>
        public string CacheDirectory { get; set; } = ".synapse/genes";
        
        /// <summary>
        /// Gemini API Key
        /// </summary>
        public string? GeminiApiKey { get; set; }
        
        /// <summary>
        /// Gemini API Endpoint
        /// </summary>
        public string? GeminiEndpoint { get; set; }
    }
}
