using System;
using System.Diagnostics;
using Synapse.Core.Services;

namespace Synapse.Core.Telemetry
{
    /// <summary>
    /// 性能追踪器 - 使用 using 语句自动追踪方法执行时间
    /// </summary>
    public sealed class PerformanceTracker : IDisposable
    {
        private readonly Stopwatch _stopwatch;
        private readonly string _methodName;
        private readonly string _geneId;
        private readonly IMetricsCollector _collector;
        private bool _disposed;
        private bool _failed;
        
        public PerformanceTracker(
            string methodName,
            string geneId,
            IMetricsCollector collector)
        {
            _methodName = methodName;
            _geneId = geneId;
            _collector = collector;
            _stopwatch = Stopwatch.StartNew();
        }
        
        /// <summary>
        /// 标记执行失败
        /// </summary>
        public void MarkFailed()
        {
            _failed = true;
        }
        
        public void Dispose()
        {
            if (_disposed) return;
            
            _stopwatch.Stop();
            var elapsedMs = _stopwatch.Elapsed.TotalMilliseconds;
            
            _collector.RecordExecution(_methodName, _geneId, elapsedMs, !_failed);
            
            _disposed = true;
        }
    }
}
