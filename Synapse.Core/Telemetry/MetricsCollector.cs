using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Synapse.Core.Services;

namespace Synapse.Core.Telemetry
{
    /// <summary>
    /// 性能指标收集器实现
    /// </summary>
    public class MetricsCollector : IMetricsCollector
    {
        private readonly ConcurrentDictionary<string, GeneMetricsData> _metrics = new();
        
        public void RecordExecution(string methodName, string geneId, double durationMs, bool success = true)
        {
            var key = $"{methodName}:{geneId}";
            var metrics = _metrics.GetOrAdd(key, _ => new GeneMetricsData(methodName, geneId));
            
            metrics.RecordExecution(durationMs, success);
        }
        
        public IEnumerable<GeneMetrics> GetMetricsForMethod(string methodName)
        {
            return _metrics.Values
                .Where(m => m.MethodName == methodName)
                .Select(m => m.ToGeneMetrics());
        }
        
        public IEnumerable<GeneMetrics> GetAllMetrics()
        {
            return _metrics.Values.Select(m => m.ToGeneMetrics());
        }
        
        public void Clear()
        {
            _metrics.Clear();
        }
        
        /// <summary>
        /// 内部数据结构，用于高效收集指标
        /// </summary>
        private class GeneMetricsData
        {
            private readonly object _lock = new();
            private readonly List<double> _executionTimes = new();
            
            public string MethodName { get; }
            public string GeneId { get; }
            public long TotalExecutions { get; private set; }
            public long SuccessCount { get; private set; }
            public long FailureCount { get; private set; }
            
            public GeneMetricsData(string methodName, string geneId)
            {
                MethodName = methodName;
                GeneId = geneId;
            }
            
            public void RecordExecution(double durationMs, bool success)
            {
                lock (_lock)
                {
                    TotalExecutions++;
                    if (success)
                        SuccessCount++;
                    else
                        FailureCount++;
                    
                    _executionTimes.Add(durationMs);
                    
                    // 只保留最近 1000 次记录
                    if (_executionTimes.Count > 1000)
                    {
                        _executionTimes.RemoveAt(0);
                    }
                }
            }
            
            public GeneMetrics ToGeneMetrics()
            {
                lock (_lock)
                {
                    if (_executionTimes.Count == 0)
                    {
                        return new GeneMetrics
                        {
                            MethodName = MethodName,
                            GeneId = GeneId,
                            TotalExecutions = TotalExecutions,
                            SuccessCount = SuccessCount,
                            FailureCount = FailureCount
                        };
                    }
                    
                    var sorted = _executionTimes.OrderBy(x => x).ToList();
                    
                    return new GeneMetrics
                    {
                        MethodName = MethodName,
                        GeneId = GeneId,
                        TotalExecutions = TotalExecutions,
                        SuccessCount = SuccessCount,
                        FailureCount = FailureCount,
                        AverageTime = _executionTimes.Average(),
                        MinTime = _executionTimes.Min(),
                        MaxTime = _executionTimes.Max(),
                        P50 = GetPercentile(sorted, 50),
                        P95 = GetPercentile(sorted, 95),
                        P99 = GetPercentile(sorted, 99),
                        SuccessRate = TotalExecutions > 0 ? (double)SuccessCount / TotalExecutions * 100 : 0
                    };
                }
            }
            
            private static double GetPercentile(List<double> sorted, int percentile)
            {
                if (sorted.Count == 0) return 0;
                
                int index = (int)Math.Ceiling(percentile / 100.0 * sorted.Count) - 1;
                index = Math.Max(0, Math.Min(index, sorted.Count - 1));
                
                return sorted[index];
            }
        }
    }
}
