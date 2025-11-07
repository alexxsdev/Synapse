using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Synapse.Core.Evolution
{
    /// <summary>
    /// 基因缓存接口
    /// </summary>
    public interface IGeneCache
    {
        void SaveGene(string geneId, byte[] assemblyBytes, string sourceCode, Dictionary<string, object> metadata);
        (bool success, Assembly? assembly, string sourceCode) LoadGene(string geneId);
        void LoadAllCachedGenes(IDynamicGeneLoader loader);
        List<string> GetCachedGenes();
        void DeleteGene(string geneId);
    }
    
    /// <summary>
    /// 基因缓存 - 持久化 AI 生成的代码
    /// </summary>
    public class GeneCache : IGeneCache
    {
        private readonly string _cacheDirectory;
        private const string MetadataFileName = "genes.json";
        
        public GeneCache(string cacheDirectory = ".synapse/genes")
        {
            _cacheDirectory = Path.GetFullPath(cacheDirectory);
            Directory.CreateDirectory(_cacheDirectory);
        }
        
        public void SaveGene(string geneId, byte[] assemblyBytes, string sourceCode, Dictionary<string, object> metadata)
        {
            try
            {
                // 保存程序集
                var assemblyPath = Path.Combine(_cacheDirectory, $"{geneId}.dll");
                File.WriteAllBytes(assemblyPath, assemblyBytes);
                
                // 保存源代码
                var sourcePath = Path.Combine(_cacheDirectory, $"{geneId}.cs");
                File.WriteAllText(sourcePath, sourceCode);
                
                // 更新元数据
                var allMetadata = LoadMetadata();
                allMetadata[geneId] = new GeneMetadata
                {
                    GeneId = geneId,
                    CreatedAt = DateTime.Now,
                    SourceCode = sourceCode,
                    AssemblyPath = assemblyPath,
                    Metadata = metadata
                };
                SaveMetadata(allMetadata);
                
                Console.WriteLine($"💾 [基因缓存] 基因已保存: {geneId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [基因缓存] 保存失败: {ex.Message}");
            }
        }
        
        public (bool success, Assembly? assembly, string sourceCode) LoadGene(string geneId)
        {
            try
            {
                var assemblyPath = Path.Combine(_cacheDirectory, $"{geneId}.dll");
                var sourcePath = Path.Combine(_cacheDirectory, $"{geneId}.cs");
                
                if (!File.Exists(assemblyPath))
                {
                    return (false, null, string.Empty);
                }
                
                var assemblyBytes = File.ReadAllBytes(assemblyPath);
                var assembly = Assembly.Load(assemblyBytes);
                
                var sourceCode = File.Exists(sourcePath) ? File.ReadAllText(sourcePath) : string.Empty;
                
                return (true, assembly, sourceCode);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [基因缓存] 加载失败: {ex.Message}");
                return (false, null, string.Empty);
            }
        }
        
        public void LoadAllCachedGenes(IDynamicGeneLoader loader)
        {
            try
            {
                var metadata = LoadMetadata();
                
                if (metadata.Count == 0)
                {
                    return;
                }
                
                Console.WriteLine($"📦 [基因缓存] 发现 {metadata.Count} 个缓存的基因");
                
                foreach (var (geneId, _) in metadata)
                {
                    var (success, assembly, _) = LoadGene(geneId);
                    
                    if (success && assembly != null)
                    {
                        // 查找方法并注册
                        var type = assembly.GetTypes().FirstOrDefault(t => t.Name.Contains("Generated"));
                        if (type != null)
                        {
                            var method = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                .FirstOrDefault(m => !m.IsSpecialName);
                            
                            if (method != null)
                            {
                                var instance = Activator.CreateInstance(type);
                                loader.RegisterGene(geneId, method, instance!);
                                Console.WriteLine($"✅ [基因缓存] 已加载: {geneId}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [基因缓存] 加载失败: {ex.Message}");
            }
        }
        
        public List<string> GetCachedGenes()
        {
            return LoadMetadata().Keys.ToList();
        }
        
        public void DeleteGene(string geneId)
        {
            try
            {
                var assemblyPath = Path.Combine(_cacheDirectory, $"{geneId}.dll");
                var sourcePath = Path.Combine(_cacheDirectory, $"{geneId}.cs");
                
                if (File.Exists(assemblyPath)) File.Delete(assemblyPath);
                if (File.Exists(sourcePath)) File.Delete(sourcePath);
                
                var metadata = LoadMetadata();
                metadata.Remove(geneId);
                SaveMetadata(metadata);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [基因缓存] 删除失败: {ex.Message}");
            }
        }
        
        private Dictionary<string, GeneMetadata> LoadMetadata()
        {
            var metadataPath = Path.Combine(_cacheDirectory, MetadataFileName);
            
            if (!File.Exists(metadataPath))
            {
                return new Dictionary<string, GeneMetadata>();
            }
            
            try
            {
                var json = File.ReadAllText(metadataPath);
                return JsonSerializer.Deserialize<Dictionary<string, GeneMetadata>>(json) 
                    ?? new Dictionary<string, GeneMetadata>();
            }
            catch
            {
                return new Dictionary<string, GeneMetadata>();
            }
        }
        
        private void SaveMetadata(Dictionary<string, GeneMetadata> metadata)
        {
            var metadataPath = Path.Combine(_cacheDirectory, MetadataFileName);
            var json = JsonSerializer.Serialize(metadata, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(metadataPath, json);
        }
        
        private class GeneMetadata
        {
            public string GeneId { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public string SourceCode { get; set; } = string.Empty;
            public string AssemblyPath { get; set; } = string.Empty;
            public Dictionary<string, object> Metadata { get; set; } = new();
        }
    }
}
