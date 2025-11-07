using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Synapse.AI.Services
{
    public class RoslynCodeCompiler : ICodeCompiler
    {
        public (bool success, string error, MethodInfo? method, byte[]? assemblyBytes) 
            CompileCode(string code, string geneName)
        {
            try
            {
                Console.WriteLine($"⚙️  [编译器] 正在编译: {geneName}");
                
                var fullCode = $@"
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Synapse.Generated
{{
    public class {geneName}_Generated
    {{
        {code}
    }}
}}";
                
                var syntaxTree = CSharpSyntaxTree.ParseText(fullCode);
                var references = GetReferences();
                
                var compilation = CSharpCompilation.Create(
                    $"DynamicGene_{geneName}_{Guid.NewGuid():N}",
                    new[] { syntaxTree },
                    references,
                    new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
                
                using var ms = new MemoryStream();
                var result = compilation.Emit(ms);
                
                if (!result.Success)
                {
                    var errors = string.Join("\n", result.Diagnostics
                        .Where(d => d.Severity == DiagnosticSeverity.Error)
                        .Select(d => $"   • {d.GetMessage()}"));
                    
                    Console.WriteLine($"❌ [编译器] 失败:\n{errors}");
                    return (false, errors, null, null);
                }
                
                var assemblyBytes = ms.ToArray();
                var assembly = Assembly.Load(assemblyBytes);
                var type = assembly.GetType($"Synapse.Generated.{geneName}_Generated");
                
                if (type == null)
                {
                    return (false, "无法找到生成的类型", null, null);
                }
                
                var method = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .FirstOrDefault(m => !m.IsSpecialName && !m.IsConstructor);
                
                if (method == null)
                {
                    return (false, "无法找到方法", null, null);
                }
                
                Console.WriteLine($"✅ [编译器] 成功: {method.Name}");
                
                return (true, string.Empty, method, assemblyBytes);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [编译器] 异常: {ex.Message}");
                return (false, ex.Message, null, null);
            }
        }
        
        private static MetadataReference[] GetReferences()
        {
            var references = new List<MetadataReference>
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
            };
            
            try
            {
                references.Add(MetadataReference.CreateFromFile(Assembly.Load("System.Runtime").Location));
                references.Add(MetadataReference.CreateFromFile(Assembly.Load("System.Collections").Location));
                references.Add(MetadataReference.CreateFromFile(Assembly.Load("System.Threading.Tasks").Location));
            }
            catch
            {
                // 忽略
            }
            
            return references.ToArray();
        }
    }
}
