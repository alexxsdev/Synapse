using System.Collections.Generic;

namespace Synapse.Core.Services
{
    /// <summary>
    /// 性能指标收集器接口
    /// </summary>
    public interface IMetricsCollector
    {
        /// <summary>
        /// 记录方法执行
        /// </summary>
        void RecordExecution(string methodName, string geneId, double durationMs, bool success = true);
        
        /// <summary>
        /// 获取方法的所有基因指标
        /// </summary>
        IEnumerable<GeneMetrics> GetMetricsForMethod(string methodName);
        
        /// <summary>
        /// 获取所有指标
        /// </summary>
        IEnumerable<GeneMetrics> GetAllMetrics();
        
        /// <summary>
        /// 清除指标
        /// </summary>
        void Clear();
    }
    
    /// <summary>
    /// 基因性能指标
    /// </summary>
    public class GeneMetrics
    {
        public string GeneId { get; set; } = string.Empty;
        public string MethodName { get; set; } = string.Empty;
        public long TotalExecutions { get; set; }
        public long SuccessCount { get; set; }
        public long FailureCount { get; set; }
        public double AverageTime { get; set; }
        public double MinTime { get; set; }
        public double MaxTime { get; set; }
        public double P50 { get; set; }
        public double P95 { get; set; }
        public double P99 { get; set; }
        public double SuccessRate { get; set; }
    }
}
