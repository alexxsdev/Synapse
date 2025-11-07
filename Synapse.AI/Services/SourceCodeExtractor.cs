using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Synapse.AI.Services
{
    /// <summary>
    /// 源代码提取器 - 从项目中提取方法的实际源代码
    /// </summary>
    public class SourceCodeExtractor
    {
        private readonly string _sourceCodePath;
        private readonly ILogger<SourceCodeExtractor> _logger;

        public SourceCodeExtractor(string sourceCodePath, ILogger<SourceCodeExtractor> logger)
        {
            _sourceCodePath = sourceCodePath;
            _logger = logger;
        }

        /// <summary>
        /// 提取方法的源代码
        /// </summary>
        public string? ExtractMethodSource(string methodName)
        {
            try
            {
                _logger.LogInformation("🔍 正在搜索方法源代码: {MethodName}", methodName);

                // 搜索所有 C# 文件
                var csFiles = Directory.GetFiles(_sourceCodePath, "*.cs", SearchOption.AllDirectories)
                    .Where(f => !f.Contains("\\obj\\") && !f.Contains("\\bin\\"))
                    .ToList();

                foreach (var file in csFiles)
                {
                    try
                    {
                        var content = File.ReadAllText(file);
                        
                        // 查找方法定义
                        var methodPattern = $@"(private|public|protected|internal)\s+(\w+\s+)?async\s+Task\s+{Regex.Escape(methodName)}\s*\([^)]*\)";
                        var match = Regex.Match(content, methodPattern, RegexOptions.Singleline);
                        
                        if (match.Success)
                        {
                            _logger.LogInformation("✅ 找到方法源代码: {File}", Path.GetFileName(file));
                            
                            // 提取完整方法（从方法签名到结束的大括号）
                            var methodSource = ExtractCompleteMethod(content, match.Index);
                            
                            if (!string.IsNullOrEmpty(methodSource))
                            {
                                // 同时提取 using 语句
                                var usings = ExtractUsings(content);
                                
                                return $@"// 文件: {Path.GetFileName(file)}
{usings}

{methodSource}";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "读取文件失败: {File}", file);
                    }
                }

                _logger.LogWarning("❌ 未找到方法源代码: {MethodName}", methodName);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "提取源代码失败");
                return null;
            }
        }

        private string ExtractCompleteMethod(string content, int startIndex)
        {
            // 找到方法开始的位置
            var methodStart = content.LastIndexOf('\n', startIndex) + 1;
            
            // 找到第一个开括号
            var openBraceIndex = content.IndexOf('{', startIndex);
            if (openBraceIndex == -1) return string.Empty;

            // 匹配括号，找到方法结束
            int braceCount = 1;
            int currentIndex = openBraceIndex + 1;
            
            while (currentIndex < content.Length && braceCount > 0)
            {
                if (content[currentIndex] == '{') braceCount++;
                else if (content[currentIndex] == '}') braceCount--;
                currentIndex++;
            }

            if (braceCount == 0)
            {
                return content.Substring(methodStart, currentIndex - methodStart).Trim();
            }

            return string.Empty;
        }

        private string ExtractUsings(string content)
        {
            var usingPattern = @"using\s+[\w\.]+;";
            var matches = Regex.Matches(content, usingPattern);
            
            var usings = string.Join("\n", matches.Select(m => m.Value));
            return usings;
        }

        /// <summary>
        /// 获取类的完整上下文（包括字段、属性等）
        /// </summary>
        public string? ExtractClassContext(string methodName)
        {
            try
            {
                var csFiles = Directory.GetFiles(_sourceCodePath, "*.cs", SearchOption.AllDirectories)
                    .Where(f => !f.Contains("\\obj\\") && !f.Contains("\\bin\\"))
                    .ToList();

                foreach (var file in csFiles)
                {
                    var content = File.ReadAllText(file);
                    
                    // 简单检查是否包含方法
                    if (content.Contains($"{methodName}("))
                    {
                        // 提取类定义
                        var classPattern = @"(public|internal)\s+class\s+(\w+)[^{]*\{";
                        var match = Regex.Match(content, classPattern);
                        
                        if (match.Success)
                        {
                            var className = match.Groups[2].Value;
                            
                            // 提取类的字段和构造函数
                            var classInfo = ExtractClassFields(content);
                            
                            return $@"// 类: {className}
// 文件: {Path.GetFileName(file)}

{classInfo}";
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "提取类上下文失败");
                return null;
            }
        }

        private string ExtractClassFields(string content)
        {
            var lines = content.Split('\n');
            var fields = lines
                .Where(l => l.Contains("private readonly") || l.Contains("private "))
                .Take(20)
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l));

            return string.Join("\n", fields);
        }
    }
}

