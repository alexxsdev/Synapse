using System;

namespace Synapse.Core.Attributes
{
    /// <summary>
    /// 标记方法为可进化的，框架会自动监控和优化此方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class EvolvableAttribute : Attribute
    {
        /// <summary>
        /// 优化目标（可选）
        /// </summary>
        public string? Goal { get; set; }
        
        /// <summary>
        /// 性能阈值（毫秒），超过此值触发优化
        /// </summary>
        public double PerformanceThreshold { get; set; } = 100.0;
        
        /// <summary>
        /// 是否启用 AI 自动生成
        /// </summary>
        public bool EnableAI { get; set; } = true;
        
        /// <summary>
        /// A/B 测试流量百分比（0-100）
        /// </summary>
        public int ABTestPercentage { get; set; } = 0;
        
        public EvolvableAttribute()
        {
        }
        
        public EvolvableAttribute(string goal)
        {
            Goal = goal;
        }
    }
}
