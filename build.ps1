# Synapse Framework 构建脚本

Write-Host "🧬 开始构建 Synapse Framework..." -ForegroundColor Cyan

# 清理
Write-Host "`n🧹 清理旧文件..." -ForegroundColor Yellow
dotnet clean

# 恢复依赖
Write-Host "`n📦 恢复 NuGet 包..." -ForegroundColor Yellow
dotnet restore

# 构建 Core
Write-Host "`n🔨 构建 Synapse.Core..." -ForegroundColor Yellow
dotnet build Synapse.Core/Synapse.Core.csproj -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Synapse.Core 构建失败" -ForegroundColor Red
    exit 1
}

# 构建 AI
Write-Host "`n🔨 构建 Synapse.AI..." -ForegroundColor Yellow
dotnet build Synapse.AI/Synapse.AI.csproj -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Synapse.AI 构建失败" -ForegroundColor Red
    exit 1
}

# 构建示例
Write-Host "`n🔨 构建示例项目..." -ForegroundColor Yellow
dotnet build Examples/WebApi.Example/WebApi.Example.csproj -c Release
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ 示例项目构建失败" -ForegroundColor Red
    exit 1
}

Write-Host "`n✅ 构建完成！" -ForegroundColor Green
Write-Host "`n📦 输出目录:" -ForegroundColor Cyan
Write-Host "   • Synapse.Core: Synapse.Core/bin/Release/net8.0/"
Write-Host "   • Synapse.AI: Synapse.AI/bin/Release/net8.0/"
Write-Host "   • 示例: Examples/WebApi.Example/bin/Release/net8.0/"

Write-Host "`n🚀 运行示例:" -ForegroundColor Cyan
Write-Host "   cd Examples/WebApi.Example"
Write-Host "   dotnet run"
