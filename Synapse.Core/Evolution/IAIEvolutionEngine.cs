using System.Threading.Tasks;

namespace Synapse.Core.Evolution
{
    /// <summary>
    /// AI 进化引擎接口 - 负责使用 AI 生成优化代码
    /// </summary>
    public interface IAIEvolutionEngine
    {
        /// <summary>
        /// 分析当前性能数据并使用 AI 生成优化代码
        /// </summary>
        Task AnalyzeAndEvolveAsync();
    }
}

