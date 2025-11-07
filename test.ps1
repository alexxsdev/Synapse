# Synapse Framework 测试脚本

param(
    [int]$Requests = 30,
    [string]$Url = "http://localhost:5000/api/product/search?query=Pro"
)

Write-Host "🧪 Synapse 自动化测试" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

# 检查示例项目是否在运行
Write-Host "`n🔍 检查服务状态..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri $Url -Method GET -TimeoutSec 5 -ErrorAction Stop
    Write-Host "✅ 服务正在运行" -ForegroundColor Green
} catch {
    Write-Host "❌ 服务未运行，请先启动示例项目:" -ForegroundColor Red
    Write-Host "   cd Examples/WebApi.Example" -ForegroundColor Yellow
    Write-Host "   dotnet run" -ForegroundColor Yellow
    exit 1
}

# 发送测试请求
Write-Host "`n📊 发送 $Requests 个测试请求..." -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

$times = @()
$successCount = 0
$failCount = 0

for ($i = 1; $i -le $Requests; $i++) {
    $progress = [math]::Round(($i / $Requests) * 100)
    Write-Progress -Activity "发送请求" -Status "$i/$Requests" -PercentComplete $progress
    
    try {
        $sw = [System.Diagnostics.Stopwatch]::StartNew()
        $response = Invoke-WebRequest -Uri $Url -Method GET -TimeoutSec 10
        $sw.Stop()
        
        $times += $sw.ElapsedMilliseconds
        $successCount++
        
        if ($i % 10 -eq 0) {
            Write-Host "  ✓ 完成 $i 个请求" -ForegroundColor Green
        }
    } catch {
        $failCount++
        Write-Host "  ✗ 请求 $i 失败: $($_.Exception.Message)" -ForegroundColor Red
    }
    
    Start-Sleep -Milliseconds 100
}

Write-Progress -Activity "发送请求" -Completed

# 计算统计数据
Write-Host "`n📈 性能统计" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

if ($times.Count -gt 0) {
    $sortedTimes = $times | Sort-Object
    $avg = ($times | Measure-Object -Average).Average
    $min = ($times | Measure-Object -Minimum).Minimum
    $max = ($times | Measure-Object -Maximum).Maximum
    $p50 = $sortedTimes[[math]::Floor($sortedTimes.Count * 0.5)]
    $p95 = $sortedTimes[[math]::Floor($sortedTimes.Count * 0.95)]
    $p99 = $sortedTimes[[math]::Floor($sortedTimes.Count * 0.99)]
    
    Write-Host "  总请求数: $Requests" -ForegroundColor White
    Write-Host "  成功: $successCount" -ForegroundColor Green
    Write-Host "  失败: $failCount" -ForegroundColor $(if ($failCount -gt 0) { "Red" } else { "Gray" })
    Write-Host "  成功率: $([math]::Round(($successCount / $Requests) * 100, 2))%" -ForegroundColor White
    Write-Host ""
    Write-Host "  平均响应时间: $([math]::Round($avg, 2))ms" -ForegroundColor White
    Write-Host "  最小响应时间: ${min}ms" -ForegroundColor Green
    Write-Host "  最大响应时间: ${max}ms" -ForegroundColor Yellow
    Write-Host "  P50 (中位数): ${p50}ms" -ForegroundColor White
    Write-Host "  P95: ${p95}ms" -ForegroundColor Yellow
    Write-Host "  P99: ${p99}ms" -ForegroundColor Red
}

# 检查是否触发了 AI 生成
Write-Host "`n🤖 检查 AI 进化状态..." -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

$genesPath = "Examples/WebApi.Example/.synapse/genes"
if (Test-Path $genesPath) {
    $geneFiles = Get-ChildItem -Path $genesPath -Filter "*.cs"
    
    if ($geneFiles.Count -gt 0) {
        Write-Host "  ✅ 发现 $($geneFiles.Count) 个生成的基因:" -ForegroundColor Green
        foreach ($file in $geneFiles) {
            Write-Host "     • $($file.Name)" -ForegroundColor White
        }
        
        Write-Host "`n  💡 查看生成的代码:" -ForegroundColor Yellow
        Write-Host "     cat $genesPath/$($geneFiles[0].Name)" -ForegroundColor Gray
    } else {
        Write-Host "  ⏳ 还未生成新基因（需要更多请求或更差的性能）" -ForegroundColor Yellow
        Write-Host "     • 当前阈值: 20 次执行" -ForegroundColor Gray
        Write-Host "     • 性能阈值: 50ms" -ForegroundColor Gray
    }
} else {
    Write-Host "  ⏳ 基因缓存目录不存在，还未触发 AI 生成" -ForegroundColor Yellow
}

Write-Host "`n✅ 测试完成！" -ForegroundColor Green
Write-Host "`n💡 提示:" -ForegroundColor Cyan
Write-Host "   • 查看服务日志以了解详细的进化过程" -ForegroundColor Gray
Write-Host "   • 继续发送请求以触发 AI 优化" -ForegroundColor Gray
Write-Host "   • 修改 appsettings.json 调整进化参数" -ForegroundColor Gray
