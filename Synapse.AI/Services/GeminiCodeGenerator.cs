using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Synapse.Core.Services;

namespace Synapse.AI.Services
{
    public class GeminiCodeGenerator : ICodeGenerator
    {
        private readonly HttpClient _httpClient;
        private readonly AIOptions _options;
        
        public GeminiCodeGenerator(AIOptions options)
        {
            _options = options;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(60);
        }
        
        public async Task<string> GenerateOptimizedCodeAsync(
            string currentCode,
            GeneMetrics metrics,
            string optimizationGoal)
        {
            try
            {
                Console.WriteLine("🤖 [AI] 正在生成优化代码...");
                
                var prompt = BuildPrompt(currentCode, metrics, optimizationGoal);
                var response = await CallGeminiAPI(prompt);
                var code = ExtractCode(response);
                
                Console.WriteLine("✅ [AI] 代码生成完成");
                return code;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [AI] 生成失败: {ex.Message}");
                return string.Empty;
            }
        }
        
        private string BuildPrompt(string code, GeneMetrics metrics, string goal)
        {
            return $@"你是 C# 性能优化专家。

当前代码:
```csharp
{code}
```

性能数据:
- P95: {metrics.P95:F2}ms
- 平均: {metrics.AverageTime:F2}ms
- 执行次数: {metrics.TotalExecutions}

优化目标: {goal}

请生成优化后的代码，要求:
1. 保持方法签名不变
2. 不添加访问修饰符
3. 只返回方法代码

优化后的代码:";
        }
        
        private async Task<string> CallGeminiAPI(string prompt)
        {
            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                },
                generationConfig = new
                {
                    temperature = 0.3,
                    maxOutputTokens = 2000
                }
            };
            
            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var url = $"{_options.Endpoint}?key={_options.ApiKey}";
            var response = await _httpClient.PostAsync(url, content);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"API 失败: {response.StatusCode} - {error}");
            }
            
            var responseContent = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonDocument.Parse(responseContent);
            
            return jsonResponse.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? string.Empty;
        }
        
        private string ExtractCode(string response)
        {
            var startMarker = "```csharp";
            var endMarker = "```";
            
            var startIndex = response.IndexOf(startMarker);
            if (startIndex == -1) return response.Trim();
            
            startIndex += startMarker.Length;
            var endIndex = response.IndexOf(endMarker, startIndex);
            
            if (endIndex == -1) return response.Substring(startIndex).Trim();
            
            return response.Substring(startIndex, endIndex - startIndex).Trim();
        }
    }
}
