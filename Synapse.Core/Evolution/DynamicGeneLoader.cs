using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Synapse.Core.Evolution
{
    /// <summary>
    /// 动态基因加载器接口
    /// </summary>
    public interface IDynamicGeneLoader
    {
        void RegisterGene(string geneId, MethodInfo method, object instance);
        TResult? ExecuteGene<TResult>(string geneId, params object[] parameters);
        bool HasGene(string geneId);
        IEnumerable<string> GetAllGenes();
    }
    
    /// <summary>
    /// 动态基因加载器 - 运行时加载和执行 AI 生成的代码
    /// </summary>
    public class DynamicGeneLoader : IDynamicGeneLoader
    {
        private readonly ConcurrentDictionary<string, GeneDelegate> _genes = new();
        
        public void RegisterGene(string geneId, MethodInfo method, object instance)
        {
            _genes[geneId] = new GeneDelegate(method, instance);
        }
        
        public TResult? ExecuteGene<TResult>(string geneId, params object[] parameters)
        {
            if (!_genes.TryGetValue(geneId, out var gene))
            {
                return default;
            }
            
            try
            {
                var result = gene.Method.Invoke(gene.Instance, parameters);
                return (TResult?)result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [动态基因] 执行失败: {ex.InnerException?.Message ?? ex.Message}");
                return default;
            }
        }
        
        public bool HasGene(string geneId)
        {
            return _genes.ContainsKey(geneId);
        }
        
        public IEnumerable<string> GetAllGenes()
        {
            return _genes.Keys;
        }
        
        private class GeneDelegate
        {
            public MethodInfo Method { get; }
            public object Instance { get; }
            
            public GeneDelegate(MethodInfo method, object instance)
            {
                Method = method;
                Instance = instance;
            }
        }
    }
}
