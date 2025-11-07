using System.Threading.Tasks;
using Synapse.Core.Services;

namespace Synapse.AI.Services
{
    /// <summary>
    /// 代码生成器接口
    /// </summary>
    public interface ICodeGenerator
    {
        Task<string> GenerateOptimizedCodeAsync(
            string currentCode,
            GeneMetrics metrics,
            string optimizationGoal);
    }
    
    /// <summary>
    /// 代码编译器接口
    /// </summary>
    public interface ICodeCompiler
    {
        (bool success, string error, System.Reflection.MethodInfo? method, byte[]? assemblyBytes) 
            CompileCode(string code, string geneName);
    }
}
